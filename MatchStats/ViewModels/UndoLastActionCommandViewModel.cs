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
            var currentSet = currentMatch.Sets.FirstOrDefault(x => x.IsCurrentSet);
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

                    var game = currentMatch.CurrentGame();
                    if (game.Points.Count > 0)
                    {
                        //Remove only if the current game has points to remove - other wise it will be dealt with in the next 'If' statement below.
                        game.Points.RemoveAt(game.Points.Count - 1);
                    }

                    if (currentMatch.CurrentGame().PlayerOneScore < 0 || currentMatch.CurrentGame().PlayerTwoScore < 0) //Less than zero will indicate we have should have undo unto previous game
                    {
                        var games = currentMatch.CurrentSet().Games.Count;
                        currentMatch.CurrentSet().Games.RemoveAt(games - 1);

                        if (currentMatch.CurrentSet().Games.Count == 0)
                        {
                            // If an undo operation is for an action in a previous set, then set that set as the current set before reducing the score
                            currentMatch.Sets.RemoveAt(currentMatch.Sets.Count - 1);
                            var lastSet = currentMatch.Sets.LastOrDefault();
                            if (lastSet != null)
                            {
                                lastSet.IsCurrentSet = true;
                            }
                        }

                        if (lastMatchStat.Player.IsPlayerOne)currentMatch.CurrentSet().Games.Last().PlayerOneScore --;
                        if (! lastMatchStat.Player.IsPlayerOne) currentMatch.CurrentSet().Games.Last().PlayerTwoScore--;
                        var current  = currentMatch.CurrentSet().Games.Last();
                        if (current.Points.Count > 0)
                        {
                            current.Points.RemoveAt(current.Points.Count - 1);
                        }

                        //Switch players serving
                        currentMatch.CurrentServer = currentMatch.CurrentServer.IsPlayerOne ? currentMatch.PlayerTwo : currentMatch.PlayerOne;
                        currentMatch.CurrentSet().Games.Last().IsCurrentGame = true;
                        currentMatch.CurrentSet().Games.Last().Winner = null;

                        //The status will be the current MatchStat.
                        if (currentMatch.MatchStats.Count >= 2)
                        {
                            var status = currentMatch.MatchStats[currentMatch.MatchStats.Count - 2].Reason;
                            switch (status)
                            {
                                case StatDescription.BreakPoint:
                                    currentMatch.CurrentSet().Games.Last().GameStatus.Status = Status.BreakPoint;
                                    break;
                                case StatDescription.GamePoint:
                                    currentMatch.CurrentSet().Games.Last().GameStatus.Status = Status.GamePoint;
                                    break;
                                default:
                                    bool isGamePoint;
                                    if (currentMatch.CurrentServer.IsPlayerOne)
                                    {
                                        isGamePoint = currentMatch.CurrentGame().PlayerOneScore > currentMatch.CurrentGame().PlayerTwoScore;
                                    }
                                    else
                                    {
                                        isGamePoint = currentMatch.CurrentGame().PlayerTwoScore > currentMatch.CurrentGame().PlayerOneScore;
                                    }
                                  
                                    currentMatch.CurrentSet().Games.Last().GameStatus.Status =  isGamePoint ? Status.GamePoint : Status.BreakPoint;
                                    break;
                            }
                        }
                    }
                }
                currentMatch.MatchStats.RemoveAt(currentMatch.MatchStats.Count - 1);
            }

            matchStatsApi.SaveMatch(currentMatch);
            MessageBus.Current.SendMessage(currentMatch, "PointUpdateForCurrentMatch");
        }

        public event EventHandler CanExecuteChanged;
    }
}
