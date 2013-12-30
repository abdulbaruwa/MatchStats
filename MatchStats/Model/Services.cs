using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Effects;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;
using Akavache;
using MatchStats.Enums;
using ReactiveUI;
using MatchStats.Model;
using WinRTXamlToolkit.Imaging;

namespace MatchStats.Model
{
    public interface IMatchStatsApi
    {
        void SaveMatchStats(List<MyMatchStats> matchStats);
        IObservable<Unit> SaveMatch(Match match);
        IObservable<List<MyMatchStats>> FetchMatchStats();
        IObservable<Match> ExecuteActionCommand(ICommand command);
        IObservable<Match> GetCurrentMatch();
        Match ApplyGameRules(Match currentMatch);
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

        public void SaveMatchStats(List<MyMatchStats> matchStats)
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

            return
                _blobCache.InsertObject("MyMatches", existingMatches)
                    .Concat(_blobCache.InsertObject("CurrentMatch", match));
        }

        public IObservable<List<MyMatchStats>> FetchMatchStats()
        {
            IObservable<List<MyMatchStats>> observableRes = _blobCache.GetObjectAsync<List<MyMatchStats>>("MyMatchStats");
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

            //Duece
            CheckDueceRule(currentMatch, ref currentGame);

            //Duece Sudden Death
            CheckGamePointOnSuddenDeathDueceRule(currentMatch, ref currentGame);

            //Is game over 
            CheckGameIsOverAndInitializeNewGameIfNeedBe(currentMatch);

            //Is Set Over
            CheckSetIsOverAndInitializeNewSetIfNeedBe(currentMatch);

            return currentMatch;
        }

        private bool CheckSetIsOverAndInitializeNewSetIfNeedBe(Match currentMatch)
        {
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
                    //Has that player won the set
                    if (playerOneGamesCount.DiffValueWith(playerTwoGamesCount) >= 2)

                    {
                        var playerOneIsWinner = playerOneGamesCount > playerTwoGamesCount;
                        currentMatch.CurrentSet().Winner = playerOneIsWinner
                            ? currentMatch.PlayerOne
                            : currentMatch.PlayerTwo;
                        currentMatch.CurrentSet().IsCurrentSet = false;
                        currentMatch.Score.Sets.Add(new Set() {IsCurrentSet = true});

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
                        currentMatch.Score.Sets.Add(new Set() { IsCurrentSet = true });

                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckGameIsOverAndInitializeNewGameIfNeedBe(Match currentMatch)
        {
            var gamesCount = (int)currentMatch.MatchFormat.SetsFormat;
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
                            currentMatch.CurrentSet().Games.Add(new Game() {IsCurrentGame = true});
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
                            currentMatch.CurrentSet().Games.Add(new Game() { IsCurrentGame = true });
                            return true;
                        }

                        // 4-4 or 6 - 6
                        if (maxGamesWon == gamesCount && playerOneGamesCount == playerTwoGamesCount)
                        {
                            currentMatch.CurrentSet().Games.Add(new Game() { IsCurrentGame = true, GameType = GameType.SevenPointer });
                            return true;
                        }
                    }
                    
                    //
                }
            }
            return false;
        }

        private static bool CheckDueceRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.GameType == GameType.Normal && currentGame.PlayerOneScore == currentGame.PlayerTwoScore && currentGame.PlayerOneScore >= 3)
            {
                if (currentMatch.MatchFormat.DueceFormat == DueceFormat.SuddenDeath)
                {
                    //It is Game point to either player
                    currentGame.GameStatus.Status = Status.GamePoint;
                }
                else
                {
                    currentGame.GameStatus.Status = Status.Duece;
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
                        currentGame.GameStatus = new GameStatus
                        {
                            Status = Status.GameOver,
                            Player = currentMatch.PlayerOne
                        };
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
                        currentGame.GameStatus = new GameStatus
                        {
                            Status = Status.GameOver,
                            Player = currentMatch.PlayerTwo
                        };
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CheckAdvantageRule(Match currentMatch, ref Game currentGame)
        {
            if (currentGame.PlayerOneScore >= 3 && currentGame.PlayerTwoScore >= 3)
            {
                if (currentGame.PlayerOneScore == currentGame.PlayerTwoScore + 1)
                {
                    //Advantage to playerOne
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.Advantage,
                        Player = currentMatch.PlayerOne
                    };
                    return true;
                }
                if (currentGame.PlayerTwoScore == currentGame.PlayerOneScore + 1)
                {
                    //Advantage to PlayerTwo
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.Advantage,
                        Player = currentMatch.PlayerTwo
                    };
                    return true;
                }
            }
            return false;
        }

        private static bool CheckBreakPointForPlayerTwoRule(Match currentMatch, ref Game currentGame)
        {
            if ((currentGame.PlayerOneScore == 3) && (currentGame.PlayerTwoScore <= 2))
            {
                if (currentMatch.Score.CurrentServer.IsPlayerOne)
                {
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GamePoint,
                        Player = currentMatch.PlayerOne
                    };
                    return true;
                }
                else
                {
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.BreakPoint,
                        Player = currentMatch.PlayerTwo
                    };
                    return true;
                }
            }
            return false;
        }

        private static bool CheckGamePointOnSuddenDeathDueceRule(Match currentMatch, ref Game currentGame)
        {
            if (currentMatch.MatchFormat.DueceFormat == DueceFormat.SuddenDeath)
            {
                if (currentGame.PlayerOneScore == 4)
                {
                    currentGame.Winner = currentMatch.PlayerOne;
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GameOver,
                        Player = currentMatch.PlayerOne
                    };
                    return true;
                }
                if (currentGame.PlayerTwoScore == 4)
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

        private static bool CheckBreakPointForPlayerOneRule(Match currentMatch, ref Game currentGame)
        {
            if ((currentGame.PlayerTwoScore == 3) && (currentGame.PlayerOneScore <= 2))
            {
                if (currentMatch.Score.CurrentServer.IsPlayerOne)
                {
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.BreakPoint,
                        Player = currentMatch.PlayerTwo
                    };
                    return true;
                }
                currentGame.GameStatus = new GameStatus
                {
                    Status = Status.GamePoint,
                    Player = currentMatch.PlayerTwo
                };
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