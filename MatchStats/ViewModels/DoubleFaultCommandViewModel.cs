using System;
using System.Linq;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public class UnforcedForehandErrorCommandViewModel : DoubleFaultCommandViewModel
    {
        public UnforcedForehandErrorCommandViewModel(Player player = null) : base(player)
        {
            Name = "UnforcedForehandError";
            DisplayNameTop = "Unforced";
            DisplayNameBottom = "Forehand Error";
            PointReason = PointReason.UnforcedForehandError;
        }
    }

    public class UnforcedBackhandErrorCommandViewModel : DoubleFaultCommandViewModel
    {
        public UnforcedBackhandErrorCommandViewModel(Player player = null) : base(player)
        {
            Name = "UnforcedBackhandError";
            DisplayNameTop = "Unforced";
            DisplayNameBottom = "Backhand Error";
            PointReason = PointReason.UnforcedBackhadError;
        }
    }

    public class UnforcedVolleyErrorCommandViewModel : DoubleFaultCommandViewModel
    {
        public UnforcedVolleyErrorCommandViewModel(Player player = null) : base(player)
        {
            Name = "UnforcedVolleyError";
            DisplayNameTop = "Unforced";
            DisplayNameBottom = "Volley";
            PointReason = PointReason.UnforcedVolleyError;
        }
    }
    public class ForcedErrorCommandViewModel : DoubleFaultCommandViewModel
    {
        public ForcedErrorCommandViewModel(Player player = null) : base(player)
        {
            Name = "ForcedError";
            DisplayNameTop = "Forced";
            DisplayNameBottom = "Error";
            PointReason = PointReason.ForcedError;
        }
    }

    public class DoubleFaultCommandViewModel : ReactiveObject, IGameActionViewModel
    {
        public DoubleFaultCommandViewModel(Player player = null)
        {
            Player = player ?? new Player();
            Name = "DoubleFault";
            DisplayNameTop = "Double";
            DisplayNameBottom = "Fault";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => Execute());
            PointReason = PointReason.DoubleFault;
        }

        private bool _isEnabled;
        public new bool IsEnabled
        {
            get { return _isEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }

        public IReactiveCommand ActionCommand { get; set; }

        public string Name { get; set; }
        public string DisplayNameTop { get; set; }
        public string DisplayNameBottom { get; set; }
        public Player Player { get; set; }

        public void Execute()
        {
            var matchStatsApi = RxApp.DependencyResolver.GetService<IMatchStatsApi>();
            
            //Can this be passed in when the command is called?
            matchStatsApi.GetCurrentMatch().Subscribe(currentMatch =>
            {

                Game currentGame = null;
                var currentSet = currentMatch.Sets.FirstOrDefault(x => x.IsCurrentSet);
                if (currentSet != null)
                {
                    currentGame = currentSet.Games.FirstOrDefault(x => x.IsCurrentGame);
                }

                var matchStat = new MatchStat
                {
                    PointWonLostOrNone = PointWonLostOrNone.PointWon,
                    Reason = StatDescription.DoubleFault,
                    Server = currentMatch.CurrentServer,
                    GameId = currentMatch.CurrentGame().GameId,
                    SetId = currentMatch.CurrentSet().SetId
                };

                if (currentGame != null)
                {
                    if (Player.IsPlayerOne)
                    {
                        matchStat.Player = currentMatch.PlayerTwo;
                        currentGame.PlayerTwoScore += 1;
                    }
                    else
                    {
                        matchStat.Player = currentMatch.PlayerOne;
                        currentGame.PlayerOneScore += 1;
                    }
                }

                currentMatch.CurrentGame().Points.Last().PointReason = this.PointReason;
                currentMatch.CurrentGame().Points.Last().Player = Player.IsPlayerOne ? currentMatch.PlayerTwo : currentMatch.PlayerOne;
                currentMatch.CurrentGame().Points.Last()
                    .Serves.Add(new Serve()
                    {
                        IsFirstServe = false,
                        ServeIsIn = false,
                        Server = currentMatch.CurrentServer
                    });

                currentMatch.MatchStats.Add(matchStat);
                currentMatch = matchStatsApi.ApplyGameRules(currentMatch);
                matchStatsApi.SaveMatch(currentMatch);
                MessageBus.Current.SendMessage(currentMatch, "PointUpdateForCurrentMatch");
            });
        }

        public PointReason PointReason { get; protected set; }
    }

}
