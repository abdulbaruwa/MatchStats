using System;
using System.Linq;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public class DoubleFaultCommandViewModel : ReactiveObject, IGameActionViewModel
    {
        public DoubleFaultCommandViewModel(Player player = null)
        {
            Player = player ?? new Player();
            Name = "DoubleFault";
            DisplayName = "Double Fault";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => Execute());
        }

        private bool _isEnabled;
        public new bool IsEnabled
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
            Match currentMatch = null;
            var matchStatsApi = RxApp.DependencyResolver.GetService<IMatchStatsApi>();
                //Can this be passed in when the command is called?
            matchStatsApi.GetCurrentMatch().Subscribe(x => currentMatch = x);

            Game currentGame = null;
            var currentSet = currentMatch.Score.Sets.FirstOrDefault(x => x.IsCurrentSet);
            if (currentSet != null)
            {
                currentGame = currentSet.Games.FirstOrDefault(x => x.IsCurrentGame);
            }

            var matchStat = new MatchStat
            {
                PointWonLostOrNone = PointWonLostOrNone.PointWon,
                Reason = StatDescription.DoubleFault,
                Server = currentMatch.Score.CurrentServer
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

            currentMatch.MatchStats.Add(matchStat);
            currentMatch = matchStatsApi.ApplyGameRules(currentMatch);
            matchStatsApi.SaveMatch(currentMatch);
            MessageBus.Current.SendMessage(currentMatch, "PointUpdateForCurrentMatch");

        }
    }

}
