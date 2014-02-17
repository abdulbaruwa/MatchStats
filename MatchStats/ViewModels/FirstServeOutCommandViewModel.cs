using System;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public class FirstServeOutCommandViewModel : ReactiveObject, IGameActionViewModel
    {
        public FirstServeOutCommandViewModel(Player player = null)
        {
            Player = player;
            Name = "FirstServeOut";
            DisplayNameTop = "1st Serve";
            DisplayNameBottom = "Out";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => Execute());
        }

        public string Name { get; set; }
        public string DisplayNameTop { get; set; }
        public string DisplayNameBottom { get; set; }
        public Player Player { get; set; }

        public void Execute()
        {
            //Update currentMatch for this command
            var matchStatsApi = RxApp.DependencyResolver.GetService<IMatchStatsApi>();
            //Can this be passed in when the command is called?
            matchStatsApi.GetCurrentMatch().Subscribe(currentMatch =>
            {

                Player = Player ?? currentMatch.CurrentServer;
                var matchStat = new MatchStat
                {
                    PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                    Reason = StatDescription.FirstServeOut,
                    Server = currentMatch.CurrentServer,
                    Player = currentMatch.CurrentServer,
                    GameId = currentMatch.CurrentGame().GameId,
                    SetId = currentMatch.CurrentSet().SetId
                };

                var point = new Point()
                {
                    MatchSituationBefore = currentMatch.CurrentGame().LastMatchSituation,
                    Server = currentMatch.CurrentServer
                };
                point.Serves.Add(new Serve() { IsFirstServe = true, ServeIsIn = false, Server = currentMatch.CurrentServer });
                currentMatch.CurrentGame().Points.Add(point);

                currentMatch.MatchStats.Add(matchStat);
                matchStatsApi.SaveMatch(currentMatch);
                MessageBus.Current.SendMessage(currentMatch, "NonPointUpdateForCurrentMatch");
            });
        }

        public bool IsEnabled { get; set; }
        public IReactiveCommand ActionCommand { get; set; }
    }
}
