using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Globalization.NumberFormatting;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;
using Akavache;
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
        private Stack<ICommand> undoCommands;
        private IBlobCache _blobCache;
        public MatchStatsApi(IBlobCache blocCache = null)
        {
            _blobCache = blocCache ?? RxApp.DependencyResolver.GetService<IBlobCache>("UserAccount");
            undoCommands = new Stack<ICommand>();
        }

        public void SaveMatchStats(List<MyMatchStats> matchStats)
        {
            _blobCache.InsertObject("MyMatchStats", matchStats);
            var test = _blobCache.GetAllKeys();
        }

        public IObservable<Unit> SaveMatch(Match match)
        {
            //Get match and add to it.
            var existingMatches = new List<Match>();
            var observable = _blobCache.GetObjectAsync<List<Match>>("MyMatches");
            observable.Subscribe(existingMatches.AddRange,
                ex => {/**log exceptions, the exception could be due to missing key**/ });
            var existingMatch = existingMatches.FirstOrDefault(x => x.MatchGuid == match.MatchGuid);
            if (existingMatch != null)
            {
                existingMatches.Remove(existingMatch);
            }
            existingMatches.Add(match);

            return Observable.Concat(
                            _blobCache.InsertObject<List<Match>>("MyMatches", existingMatches),
                            _blobCache.InsertObject<Match>("CurrentMatch", match));
        }

        public IObservable<List<MyMatchStats>> FetchMatchStats()
        {
            var observableRes = _blobCache.GetObjectAsync<List<MyMatchStats>>("MyMatchStats");
            return observableRes;
        }

        public IObservable<Match> ExecuteActionCommand(ICommand command)
        {
            throw new NotImplementedException();
        }

        public IObservable<Match> GetCurrentMatch()
        {
            var currentMatchObservable  = _blobCache.GetObjectAsync<Match>("CurrentMatch");
            return currentMatchObservable;
        }

        public Match ApplyGameRules(Match currentMatch)
        {
            //currentMatch
            //Is current game over?
            //Func<Game, bool> 
            var currentGame = currentMatch.Score.Games.First(x => x.IsCurrentGame);
            if (currentGame.PlayerOneScore > currentGame.PlayerTwoScore)
            {
                //PlayerOne is leading
                if ((currentGame.PlayerOneScore == 4) && (currentGame.PlayerTwoScore <= 2))
                {
                    currentGame.Winner = currentMatch.PlayerOne;
                }
            }

            //PlayerTwo Breakpoint or GamePoint
            if ((currentGame.PlayerTwoScore == 3) && (currentGame.PlayerOneScore <= 2))
            {
                if (currentMatch.Score.CurrentServer.IsPlayerOne)
                {
                    currentGame.GameStatus = new GameStatus()
                    {
                        Status = Status.BreakPoint,
                        Player = currentMatch.PlayerTwo
                    };
                }
                else
                {
                    currentGame.GameStatus = new GameStatus()
                    {
                        Status = Status.GamePoint,
                        Player = currentMatch.PlayerTwo
                    };
                }
            }

            //Player One BreakPoint or GamePoint
            if ((currentGame.PlayerOneScore == 3) && (currentGame.PlayerTwoScore <= 2))
            {
                if (currentMatch.Score.CurrentServer.IsPlayerOne)
                {
                    currentGame.GameStatus = new GameStatus()
                    {
                        Status = Status.GamePoint,
                        Player = currentMatch.PlayerOne
                    };
                }
                else
                {
                    currentGame.GameStatus = new GameStatus()
                    {
                        Status = Status.BreakPoint,
                        Player = currentMatch.PlayerOne
                    };
 
                }
            }


            //Duece
            if (currentGame.PlayerOneScore >= 3)
            {
                if (currentGame.PlayerOneScore == currentGame.PlayerTwoScore)
                {
                    currentGame.GameStatus.Status = Status.Duece;
                    currentGame.GameStatus.Player = null;
                }
            }


            return currentMatch;
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