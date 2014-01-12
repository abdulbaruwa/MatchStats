using System;
using System.Linq;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{   
    public class FirstServeInCommandViewModel : ReactiveObject, IGameActionViewModel
    {
        public FirstServeInCommandViewModel(Player player = null)
        {
            Player = player;
            Name = "FirstServeIn";
            DisplayName = "First Serve In";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => Execute());
        }

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

            Player = Player ?? currentMatch.Score.CurrentServer;
            var matchStat = new MatchStat
            {
                PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                Reason = StatDescription.FirstServeIn,
                Server = currentMatch.Score.CurrentServer,
                Player = currentMatch.Score.CurrentServer
            };

            currentMatch.MatchStats.Add(matchStat);
            matchStatsApi.SaveMatch(currentMatch);
            MessageBus.Current.SendMessage(currentMatch, "NonPointUpdateForCurrentMatch");
        }

        public bool IsEnabled { get; set; }
        public IReactiveCommand ActionCommand { get; set; }
    }
}