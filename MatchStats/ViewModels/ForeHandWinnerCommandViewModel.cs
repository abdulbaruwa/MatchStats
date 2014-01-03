using System;
using System.Linq;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public class ForeHandWinnerCommandViewModel : GameActionViewModel
    {
        public ForeHandWinnerCommandViewModel(Player player = null)
        {
            Player = player ?? new Player();
            base.Name = "ForeHandWinner";
            base.DisplayName = "Fore Hand Winner";
            ActionCommand = new ReactiveCommand();
            ActionCommand.Subscribe(x => Execute());
        }
        
        public override void Execute()
        {
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
                Reason = StatDescription.ForeHandWinner,
                Server = currentMatch.Score.CurrentServer
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

            currentMatch = matchStatsApi.ApplyGameRules(currentMatch);
            matchStatsApi.SaveMatch(currentMatch);
            MessageBus.Current.SendMessage(currentMatch, "PointUpdateForCurrentMatch");
        }
    }
}