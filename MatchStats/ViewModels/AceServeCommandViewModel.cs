using System;
using System.Linq;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public class AceServeCommandViewModel : ReactiveObject, IGameActionViewModel
    {
        private bool _isEnabled;

        public AceServeCommandViewModel(Player player = null)
        {
            Player = player ?? new Player();
            Name = "AceServe";
            DisplayName = "Ace Serve";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => Execute());
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }

        public IReactiveCommand ActionCommand { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Player Player { get; set; }

        public void Execute()
        {
            //Update currentMatch for this command
            var matchStatsApi = RxApp.DependencyResolver.GetService<IMatchStatsApi>();
            //Can this be passed in when the command is called?
            matchStatsApi.GetCurrentMatch().Subscribe(currentMatch =>
            {
                Game currentGame = null;
                Set currentSet = currentMatch.Sets.FirstOrDefault(x => x.IsCurrentSet);
                if (currentSet != null)
                {
                    currentGame = currentSet.Games.FirstOrDefault(x => x.IsCurrentGame);
                }

                var matchStat = new MatchStat
                {
                    PointWonLostOrNone = PointWonLostOrNone.PointWon,
                    Reason = DetermineIfFirstOrSecondServeAce(currentMatch),
                    Server = currentMatch.CurrentServer,
                    GameId = currentMatch.CurrentGame().GameId,
                    SetId = currentMatch.CurrentSet().SetId
                };

                if (currentGame != null)
                {
                    if (Player.IsPlayerOne)
                    {
                        matchStat.Player = currentMatch.PlayerOne;
                        currentGame.PlayerOneScore += 1;
                    }
                    else
                    {
                        matchStat.Player = currentMatch.PlayerTwo;
                        currentGame.PlayerTwoScore += 1;
                    }
                }

                if (IsFirstServe(currentMatch))
                {
                    var point = new Point
                    {
                        MatchSituationBefore = currentMatch.CurrentGame().LastMatchSituation,
                        Server = currentMatch.CurrentServer,
                        Player = Player.IsPlayerOne ? currentMatch.PlayerOne : currentMatch.PlayerTwo,
                        PointReason =  PointReason.FirstServeAce
                    };

                    point.Serves.Add(new Serve
                    {
                        IsFirstServe = IsFirstServe(currentMatch),
                        ServeIsIn = true,
                        Server = currentMatch.CurrentServer
                    });
                    currentMatch.CurrentGame().Points.Add(point);
                }
                else
                {
                    currentMatch.CurrentGame().Points.Last().PointReason = PointReason.SecondServeAce;
                    currentMatch.CurrentGame().Points.Last().Player = Player.IsPlayerOne ? currentMatch.PlayerOne : currentMatch.PlayerTwo;
                    currentMatch.CurrentGame().Points.Last().Serves.Add(new Serve
                    {
                        IsFirstServe = false,
                        ServeIsIn = true,
                        Server = currentMatch.CurrentServer
                    });

                }

                currentMatch.MatchStats.Add(matchStat);
                currentMatch = matchStatsApi.ApplyGameRules(currentMatch);
                matchStatsApi.SaveMatch(currentMatch);
                MessageBus.Current.SendMessage(currentMatch, "PointUpdateForCurrentMatch");
                MessageBus.Current.SendMessage(currentMatch, "AceServeForCurrentMatch");
            });
        }

        private StatDescription DetermineIfFirstOrSecondServeAce(Match currentMatch)
        {
            MatchStat lastStat = currentMatch.MatchStats.LastOrDefault();
            if (lastStat != null && lastStat.Reason == StatDescription.FirstServeOut &&
                lastStat.Server.FullName == currentMatch.CurrentServer.FullName)
            {
                return StatDescription.SecondServeAce;
            }
            return StatDescription.FirstServeAce;
        }

        private bool IsFirstServe(Match currentMatch)
        {
            MatchStat lastStat = currentMatch.MatchStats.LastOrDefault();
            if (lastStat != null && lastStat.Reason == StatDescription.FirstServeOut &&
                lastStat.Server.FullName == currentMatch.CurrentServer.FullName)
            {
                return false;
            }
            return true;
        }
    }
}