using System;
using System.Linq;
using System.Windows.Input;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public class UndoLastActionCommandViewModel : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
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

            var continueUndo = true;
            while (continueUndo)
            {
                if (currentMatch.MatchStats.LastOrDefault() == null) break;
                var lastMatchStat = currentMatch.MatchStats.Last();
                continueUndo = lastMatchStat.UndoPrevious;
                if (lastMatchStat.PointWonLostOrNone != PointWonLostOrNone.NotAPoint)
                {
                    if (lastMatchStat.Player.IsPlayerOne) currentMatch.CurrentGame().PlayerOneScore--;
                    if (! lastMatchStat.Player.IsPlayerOne) currentMatch.CurrentGame().PlayerTwoScore--;

                    if (currentMatch.CurrentGame().PlayerOneScore < 0 || currentMatch.CurrentGame().PlayerTwoScore < 0) //Less than zero will indicate we have should have undo unto previous game
                    {
                        var games = currentMatch.CurrentSet().Games.Count;
                        currentMatch.CurrentSet().Games.RemoveAt(games - 1);
                    }
                }
                currentMatch.MatchStats.RemoveAt(currentMatch.MatchStats.Count - 1);
            }

            currentMatch = matchStatsApi.ApplyGameRules(currentMatch);
            matchStatsApi.SaveMatch(currentMatch);
            MessageBus.Current.SendMessage(currentMatch, "PointUpdateForCurrentMatch");
        }

        public event EventHandler CanExecuteChanged;
    }
}