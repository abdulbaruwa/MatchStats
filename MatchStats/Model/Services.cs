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
            Game currentGame = currentMatch.Score.Games.First(x => x.IsCurrentGame);

            //PlayerTwo Breakpoint or GamePoint
            CheckBreakPointForPlayerOneRule(ref currentMatch, currentGame);

            //Player One BreakPoint or GamePoint
            CheckBreakPointForPlayerTwoRule(ref currentMatch, currentGame);

            //Advantage
            CheckAdvantageRule(ref currentMatch, currentGame);
            
            //Game over rules
            CheckGameOverRule(ref currentMatch, currentGame);

            //Duece
            CheckDueceRule(ref currentGame);

            //Duece Sudden Death
            CheckGamePointOnSuddenDeathDueceRule(ref currentMatch, currentGame);

            return currentMatch;
        }

        private static bool CheckDueceRule(ref Game currentGame)
        {
            if (currentGame.PlayerOneScore == currentGame.PlayerTwoScore && currentGame.PlayerOneScore >= 3)
            {
                currentGame.GameStatus.Status = Status.Duece;
                currentGame.GameStatus.Player = null; // TODO: We should set this to the player that just earned the point but it is not passed in Should refactor later
                return true;
            }
            return false;
        }

        private static bool CheckGameOverRule(ref Match currentMatch, Game currentGame)
        {
            if (currentGame.PlayerOneScore > currentGame.PlayerTwoScore)
            {
                //Player one is leading by two points after 4 points
                if (currentGame.PlayerOneScore >= 4 && currentGame.PlayerTwoScore <= (currentGame.PlayerOneScore - 2))
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
                if (currentGame.PlayerTwoScore >= 4 && currentGame.PlayerOneScore <= (currentGame.PlayerTwoScore - 2))
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

        private static bool CheckAdvantageRule(ref Match currentMatch, Game currentGame)
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

        private static bool CheckBreakPointForPlayerTwoRule(ref Match currentMatch, Game currentGame)
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

        private static bool CheckGamePointOnSuddenDeathDueceRule(ref Match currentMatch, Game currentGame)
        {
            if (currentMatch.MatchFormat.DueceFormat == DueceFormat.SuddenDeath)
            {
                if (currentGame.PlayerOneScore == 4)
                {
                    currentGame.GameStatus = new GameStatus
                    {
                        Status = Status.GameOver,
                        Player = currentMatch.PlayerOne
                    };
                    return true;
                }
                if (currentGame.PlayerTwoScore == 4)
                {
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

        private static bool CheckBreakPointForPlayerOneRule(ref Match currentMatch, Game currentGame)
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