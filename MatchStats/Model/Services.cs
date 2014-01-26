using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;
using Akavache;
using MatchStats.Enums;
using ReactiveUI;

namespace MatchStats.Model
{
    public interface IMatchStatsApi
    {
        IObservable<Unit> SaveMatch(Match match);
        IObservable<List<Match>> FetchMatchStats();
        IObservable<Match> ExecuteActionCommand(ICommand command);
        IObservable<Match> GetCurrentMatch();
        Match ApplyGameRules(Match currentMatch);
        void SaveMatchStats(List<Match> matchStats);
    }

    public class MatchStatsApi : IMatchStatsApi
    {
        private readonly IBlobCache _blobCache;
        private Stack<ICommand> undoCommands;

        public MatchStatsApi(IBlobCache blocCache = null)
        {
            _blobCache = blocCache ?? RxApp.DependencyResolver.GetService<IBlobCache>("UserAccount");
            undoCommands = new Stack<ICommand>();
        }

        public void SaveMatchStats(List<Match> matchStats)
        {
            _blobCache.InsertObject("MyMatchStats", matchStats);
            IEnumerable<string> test = _blobCache.GetAllKeys();
        }

        public IObservable<Unit> SaveMatch(Match match)
        {
            //Get match and add to it.
            var existingMatches = new List<Match>();
            IObservable<List<Match>> observable = _blobCache.GetObjectAsync<List<Match>>("MyMatches");
            observable.Subscribe(existingMatches.AddRange,
                ex =>
                {
                    /**log exceptions, the exception could be due to missing key**/
                });
            Match existingMatch = existingMatches.FirstOrDefault(x => x.MatchGuid == match.MatchGuid);
            if (existingMatch != null)
            {
                existingMatches.Remove(existingMatch);
            }
            existingMatches.Add(match);
            var newVals = new List<Match>(existingMatches);
            return

                _blobCache.InsertObject("MyMatches", newVals)
                    .Concat(_blobCache.InsertObject("CurrentMatch", match));
        }

        public IObservable<List<Match>> FetchMatchStats()
        {
            IObservable<List<Match>> observableRes = _blobCache.GetObjectAsync<List<Match>>("MyMatchStats");
              return observableRes;
        }

        public IObservable<Match> ExecuteActionCommand(ICommand command)
        {
            throw new NotImplementedException();
        }

        public IObservable<Match> GetCurrentMatch()
        {
            IObservable<Match> currentMatchObservable = _blobCache.GetObjectAsync<Match>("CurrentMatch");
            return currentMatchObservable;
        }

        public Match ApplyGameRules(Match currentMatch)
        {
            //currentMatch
            //Is current game over?
            //Func<Game, bool> 
            var currentSet = currentMatch.Score.Sets.FirstOrDefault(x => x.IsCurrentSet);
            if (currentSet == null) return currentMatch;

            Game currentGame = currentSet.Games.FirstOrDefault(x => x.IsCurrentGame);
            if (currentGame == null) return currentMatch;

            //PlayerTwo Breakpoint or GamePoint
            CheckBreakPointForPlayerOneRule(currentMatch, ref currentGame);

            //Player One BreakPoint or GamePoint
            CheckBreakPointForPlayerTwoRule(currentMatch, ref currentGame);

            //Advantage
            CheckAdvantageRule(currentMatch, ref currentGame);
            
            //Game over rules
            CheckGameOverRule(currentMatch, ref currentGame);

            //Game over Short Tie Breaker to seven
            CheckTieBreakerGameOverRule(currentMatch, ref currentGame);

            //Deuce
            CheckDeuceRule(currentMatch, ref currentGame);

            //Deuce Sudden Death
            CheckGamePointOnSuddenDeathDeuceRule(currentMatch, ref currentGame);

            //Switch Server on ChampionShip TieBreak
            CheckAndSwitchCurrentServerOnChampionShipTieBreakPoints(currentMatch, ref currentGame);

            //Is game over 
            CheckGameIsOverAndInitializeNewGameIfNeedBe(currentMatch);

            //Is Set Over
            CheckSetIsOverAndInitializeNewSetIfNeedBe(currentMatch);

            return currentMatch;
        }

        private void CheckAndSwitchCurrentServerOnChampionShipTieBreakPoints(Match currentMatch, ref Game currentGame)
        {
            if (currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak 
                && currentGame.GameType == GameType.TenPointer )
            {
                //We should switch if the total score is an odd number
                if ((currentGame.PlayerOneScore + currentGame.PlayerTwoScore)%2 == 1)
                {
                    SwitchCurrentServer(currentMatch);
                }
            }
        }


        private bool CheckMatchIsOverRule(Match currentMatch)
        {
            if (currentMatch.Score.Sets.Any(x => x.Winner == null)) return false;
            if (currentMatch.Score.Sets.Any())
            {
                var groupedSetWiners = (from n in currentMatch.Score.Sets
                    group n by n.Winner.IsPlayerOne
                    into setWinners
                    select new {WinnerIsPlayerOne = setWinners.Key, setCount = setWinners.Count()}).ToList();

                var winner = groupedSetWiners.FirstOrDefault(x => x.setCount >= 2);
                if (winner != null)
                {
                    currentMatch.Score.Winner = winner.WinnerIsPlayerOne ? currentMatch.PlayerOne : currentMatch.PlayerTwo;
                    currentMatch.Score.IsMatchOver = true;
                }
            }
            return false;
        }

        private bool CheckSetIsOverAndInitializeNewSetIfNeedBe(Match currentMatch)
        {
            if (currentMatch.Score.IsMatchOver) return true;
            var gamesCount = (int)currentMatch.MatchFormat.SetsFormat;

            var currentSetGames = currentMatch.CurrentSet().Games;
            if (currentSetGames.Any(x => x.Winner == null)) return false;
            if (currentSetGames.Any())
            {
                var groupedWinner = (from n in currentSetGames
                    group n by n.Winner.IsPlayerOne
                    into winloss
                    select new {WinnerIsPlayerOne = winloss.Key, gameCount = winloss.Count()}).ToList();

                //Get games won by each player
                var playerOneGamesCount = 0;
                    if(groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne) != null) 
                        playerOneGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne).gameCount;
                var playerTwoGamesCount = 0;
                    if(groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false) != null)
                        playerTwoGamesCount =groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false).gameCount;

                //Has any player reached 4 or 6
                if (playerOneGamesCount >= gamesCount - 1 || playerTwoGamesCount >= gamesCount - 1)
                {
                    var setsToPlay = ((int)currentMatch.MatchFormat.SetsFormat);

                    //Has that player won the set
                    if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) >= 2)

                    {
                        var playerOneIsWinner = playerOneGamesCount > playerTwoGamesCount;
                        currentMatch.CurrentSet().Winner = playerOneIsWinner
                            ? currentMatch.PlayerOne
                            : currentMatch.PlayerTwo;
                        currentMatch.CurrentSet().IsCurrentSet = false;

                        if (CheckMatchIsOverRule(currentMatch)) return true;
                        currentMatch.Score.Sets.Add(new Set() {IsCurrentSet = true});

                        if (setsToPlay.DiffValueWith(currentMatch.Score.Sets.Count) == 1 && currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
                        {
                            //This is the final set and it is a Tiebreaker                        
                            currentMatch.CurrentSet().Games.Add(new Game() { IsCurrentGame = true, GameType = GameType.TenPointer });
                            SwitchCurrentServer(currentMatch);
                        }
                        else
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                        }
        
                        return true;
                    }
                    // 4-5(5-4) or 6-7(7-6) --> Code is repetitive but simpler to read
                    if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) == 1 
                        && playerOneGamesCount >= gamesCount
                        && playerTwoGamesCount >= gamesCount)
                    {
                        var playerOneIsWinner = playerOneGamesCount > playerTwoGamesCount;
                        currentMatch.CurrentSet().Winner = playerOneIsWinner
                            ? currentMatch.PlayerOne
                            : currentMatch.PlayerTwo;
                        currentMatch.CurrentSet().IsCurrentSet = false;

                        if (CheckMatchIsOverRule(currentMatch)) return true;
                        currentMatch.Score.Sets.Add(new Set() { IsCurrentSet = true });

                        //Is this the final set?
                        if (setsToPlay.DiffValueWith(currentMatch.Score.Sets.Count) == 1 &&
                            currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
                        {
                            //This is the final set                            
                            currentMatch.CurrentSet().Games.Add(new Game() {IsCurrentGame = true, GameType = GameType.TenPointer});
                            SwitchCurrentServer(currentMatch);
                        }
                        else
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static void SwitchCurrentServer(Match currentMatch)
        {
            currentMatch.Score.CurrentServer = currentMatch.Score.CurrentServer.IsPlayerOne
                ? currentMatch.PlayerTwo
                : currentMatch.PlayerOne;
        }

        private int GetNumberOfGamesForSet(Match currentMatch)
        {
            if (currentMatch.Score.Sets.Count == 3 && currentMatch.MatchFormat.FinalSetType == FinalSetFormats.TenPointChampionShipTieBreak)
            {
                return 1;
            }
            return (int)currentMatch.MatchFormat.SetsFormat;
        }

        private bool CheckGameIsOverAndInitializeNewGameIfNeedBe(Match currentMatch)
        {
            CheckMatchIsOverRule(currentMatch);
            if (currentMatch.Score.IsMatchOver) return false;

            var gamesCount = GetNumberOfGamesForSet(currentMatch);
            var currentGame = currentMatch.CurrentGame();
            if (currentGame.GameStatus.Status == Status.GameOver)
            {
                currentGame.IsCurrentGame = false;
                var currentSetGames = currentMatch.CurrentSet().Games;
                if (currentSetGames.Any())
                {
                    //Any player that has reached the number of required games?
                    var groupedWinner = (from n in currentSetGames
                        group n by n.Winner.IsPlayerOne
                        into winloss
                        select new {WinnerIsPlayerOne = winloss.Key, gameCount = winloss.Count()}).ToList();
                    var maxGamesWon = groupedWinner.Max(x => x.gameCount);

                    if (currentMatch.CurrentSet() != null)
                    {
                        if( maxGamesWon < gamesCount)
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                            return true;
                        }

                        var playerOneGamesCount = 0;
                        if (groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne) != null) 
                             playerOneGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne).gameCount;
                        var playerTwoGamesCount = 0;
                        if(groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false) != null)     
                             playerTwoGamesCount = groupedWinner.FirstOrDefault(x => x.WinnerIsPlayerOne == false).gameCount;
                        
                        //4-3 (3-4) or 6-5 (5-6) --> We can create a new game
                        if (maxGamesWon == gamesCount && playerOneGamesCount.DiffValueWith(playerTwoGamesCount) == 1)
                        {
                            AddNewGameToCurrentSetAndSwitchCurrentServer(currentMatch);
                            return true;
                        }

                        // 4-4 or 6 - 6
                        if (maxGamesWon == gamesCount && playerOneGamesCount == playerTwoGamesCount)
                        {
                            currentMatch.CurrentSet().Games.Add(new Game() { IsCurrentGame = true, GameType = GameType.SevenPointer });
                            SwitchCurrentServer(currentMatch);
                            return true;
                        }
                    }
                    
                    //
                }
            }
            return false;
        }

        private static void AddNewGameToCurrentSetAndSwitchCurrentServer(Match currentMatch)
        {
            currentMatch.CurrentSet().Games.Add(new Game() {IsCurrentGame = true});
            SwitchCurrentServer(currentMatch);
        }

        private static bool CheckDeuceRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.GameType == GameType.Normal && currentGame.PlayerOneScore == currentGame.PlayerTwoScore && currentGame.PlayerOneScore >= 3)
            {
                if (currentMatch.MatchFormat.DeuceFormat == DeuceFormat.SuddenDeath)
                {
                    //It is Game point to either player
                    currentGame.GameStatus.Status = Status.GamePoint;
                }
                else
                {
                    currentGame.GameStatus.Status = Status.Deuce;
                }
                currentGame.GameStatus.Player = null; // TODO: We should set this to the player that just earned the point but it is not passed in Should refactor later
                return true;
            }
            return false;
        }

        private static bool CheckTieBreakerGameOverRule(Match currentMatch, ref Game currentGame)
        {

            if (currentGame.GameType == GameType.SevenPointer)
            {
                //We should switch if the total score is an odd number
                if ((currentGame.PlayerOneScore + currentGame.PlayerTwoScore)%2 == 1)
                {
                   SwitchCurrentServer(currentMatch); 
                }

                if (currentGame.PlayerOneScore >= 7)
                {
                    currentGame.Winner = currentMatch.PlayerOne;
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GameOver,
                        Player = currentMatch.PlayerOne
                    };
                    return true;
                }
                if (currentGame.PlayerTwoScore >= 7)
                {
                    currentGame.Winner = currentMatch.PlayerTwo;
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GameOver,
                        Player = currentMatch.PlayerTwo
                    };
                    return true;
                }
            }
            return false;
        }

        private static bool CheckGameOverRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.GameType == GameType.Normal)
            {
                if (currentGame.PlayerOneScore > currentGame.PlayerTwoScore)
                {
                    //Player one is leading by two points after 4 points
                    if (currentGame.PlayerOneScore >= 4 &&
                        currentGame.PlayerTwoScore <= (currentGame.PlayerOneScore - 2))
                    {
                        currentGame.Winner = currentMatch.PlayerOne;
                        SetPlayerOneAsWinnerOfGame(currentMatch, currentGame);
                        return true;
                    }
                }
                else if (currentGame.PlayerTwoScore > currentGame.PlayerOneScore)
                {
                    //Player two is leading by two points after 4 points
                    if (currentGame.PlayerTwoScore >= 4 &&
                        currentGame.PlayerOneScore <= (currentGame.PlayerTwoScore - 2))
                    {
                        currentGame.Winner = currentMatch.PlayerTwo;
                        SetPlayerTwoAsWinnerOfGame(currentMatch, currentGame);
                        return true;
                    }
                }
            }

            if (currentGame.GameType != GameType.Normal)
            {
                var gameCounts = (int) currentGame.GameType;
                if (currentGame.PlayerOneScore == gameCounts)
                {
                    SetPlayerOneAsWinnerOfGame(currentMatch, currentGame);
                    //If this is the final set -> the match is over
                    FlagSetWinnerForChampionShipTieBreak(currentMatch, currentMatch.PlayerOne);
                    return true;
                }

                if (currentGame.PlayerTwoScore == gameCounts)
                {
                    currentGame.Winner = currentMatch.PlayerTwo;
                    SetPlayerTwoAsWinnerOfGame(currentMatch, currentGame);
                    //If this is the final set -> the match is over
                    FlagSetWinnerForChampionShipTieBreak(currentMatch, currentMatch.PlayerTwo);
                    return true;
                }

            }
            return false;
        }

        private static void FlagSetWinnerForChampionShipTieBreak(Match currentMatch, Player setWinner)
        {
            if (currentMatch.Score.Sets.Count == 3)
            {
                currentMatch.CurrentSet().Winner = setWinner;
            }
        }

        private static void SetPlayerTwoAsWinnerOfGame(Match currentMatch, Game currentGame)
        {
            currentGame.Winner = currentMatch.PlayerTwo;
            currentGame.GameStatus = new GameStatus
            {
                Status = Status.GameOver,
                Player = currentMatch.PlayerTwo
            };
        }

        private static void SetPlayerOneAsWinnerOfGame(Match currentMatch, Game currentGame)
        {
            currentGame.Winner = currentMatch.PlayerOne;
            currentGame.GameStatus = new GameStatus
            {
                Status = Status.GameOver,
                Player = currentMatch.PlayerOne
            };
        }

        private static bool CheckAdvantageRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.PlayerOneScore >= 3 && currentGame.PlayerTwoScore >= 3)
            {
                if (currentGame.PlayerOneScore == currentGame.PlayerTwoScore + 1)
                {
                    //Advantage to playerOne
                    SetGameStatusForPlayer(currentMatch.PlayerOne, currentGame, Status.Advantage);
                    return true;
                }
                if (currentGame.PlayerTwoScore == currentGame.PlayerOneScore + 1)
                {
                    //Advantage to PlayerTwo
                    SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.Advantage);
                    return true;
                }
            }
            return false;
        }

        private static void SetGameStatusForPlayer(Player player, Game currentGame, Status status)
        {
            currentGame.GameStatus = new GameStatus
            {
                Status = status,
                Player = player
            };

        }

        private static bool CheckBreakPointForPlayerTwoRule(Match currentMatch, ref Game currentGame)
        {
            if ((currentGame.PlayerOneScore == 3) && (currentGame.PlayerTwoScore <= 2))
            {
                if (currentMatch.Score.CurrentServer.IsPlayerOne)
                {
                    SetGameStatusForPlayer(currentMatch.PlayerOne, currentGame, Status.GamePoint);
                    currentMatch.MatchStats.Add(new MatchStat()
                    {
                        Player = currentMatch.PlayerOne,
                        PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                        Reason = StatDescription.GamePoint,
                        Server = currentMatch.Score.CurrentServer
                    });
                    return true;
                }
                else
                {
                    SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.BreakPoint);
                    currentMatch.MatchStats.Add(new MatchStat()
                    {
                        Player = currentMatch.PlayerTwo, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.BreakPoint, Server = currentMatch.Score.CurrentServer
                    });
                    return true;
                }
            }
            return false;
        }

        private static bool CheckGamePointOnSuddenDeathDeuceRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.GameType != GameType.Normal) return false;
            if (currentMatch.MatchFormat.DeuceFormat != DeuceFormat.SuddenDeath) return false;
            if (currentGame.PlayerOneScore == 4)
            {
                currentGame.Winner = currentMatch.PlayerOne;
                SetGameStatusForPlayer(currentMatch.PlayerOne, currentGame, Status.GameOver);
                currentMatch.MatchStats.Add(new MatchStat()
                {
                    Player = currentMatch.PlayerOne,
                    PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                    Reason = StatDescription.GameOver,
                    Server = currentMatch.Score.CurrentServer
                });
                return true;
            }
            if (currentGame.PlayerTwoScore == 4)
            {
                currentGame.Winner = currentMatch.PlayerTwo;
                SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.GameOver);
                currentMatch.MatchStats.Add(new MatchStat()
                {
                    Player = currentMatch.PlayerTwo,
                    PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                    Reason = StatDescription.GameOver,
                    Server = currentMatch.Score.CurrentServer
                });
                return true;
            }
            return false;
        }

        private static bool CheckBreakPointForPlayerOneRule(Match currentMatch, ref Game currentGame)
        {
            if ((currentGame.PlayerTwoScore == 3) && (currentGame.PlayerOneScore <= 2))
            {
                if (currentMatch.Score.CurrentServer.IsPlayerOne)
                {
                    SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.BreakPoint);
                    currentMatch.MatchStats.Add(new MatchStat()
                    {
                        Player = currentMatch.PlayerTwo,
                        PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                        Reason = StatDescription.BreakPoint,
                        Server = currentMatch.Score.CurrentServer
                    });
                    return true;
                }

                SetGameStatusForPlayer(currentMatch.PlayerTwo, currentGame, Status.GamePoint);
                currentMatch.MatchStats.Add(new MatchStat()
                {
                    Player = currentMatch.PlayerTwo,
                    PointWonLostOrNone = PointWonLostOrNone.NotAPoint,
                    Reason = StatDescription.GamePoint,
                    Server = currentMatch.Score.CurrentServer
                });
                return true;
            }
            return true;
        }
    }

    public class UserService : IUserService
    {
        public async Task<string> GetCurrentUserAsync()
        {
            return await UserInformation.GetFirstNameAsync();
        }

        public async Task<BitmapImage> LoadUserImageAsync()
        {
            var bitmapImage = new BitmapImage();

            var image = UserInformation.GetAccountPicture(AccountPictureKind.SmallImage) as StorageFile;
            if (image != null)
            {
                using (IRandomAccessStreamWithContentType stream = await image.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
                //var imageStream = await image.OpenReadAsync();
            }
            return bitmapImage;
        }
    }

    public interface IUserService
    {
        Task<BitmapImage> LoadUserImageAsync();
        Task<string> GetCurrentUserAsync();
    }

    public class FakeUserSevice : IUserService
    {
        public async Task<BitmapImage> LoadUserImageAsync()
        {
            return null;
        }

        public async Task<string> GetCurrentUserAsync()
        {
            return "Abdul";
        }
    }
}
