using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        void SaveMatch(Match matches);
        IObservable<List<MyMatchStats>> FetchMatchStats();
    }

    public class MatchStatsApi : IMatchStatsApi
    {
        private IBlobCache _blobCache;
        public MatchStatsApi(IBlobCache blocCache = null)
        {
            _blobCache = blocCache ?? RxApp.DependencyResolver.GetService<IBlobCache>("UserAccount");
        }

        public void SaveMatchStats(List<MyMatchStats> matchStats)
        {
            _blobCache.InsertObject("MyMatchStats", matchStats);
            var test = _blobCache.GetAllKeys();
        }

        public void SaveMatch(Match matches)
        {
            throw new NotImplementedException();
        }

        public IObservable<List<MyMatchStats>> FetchMatchStats()
        {
            var observableRes = _blobCache.GetObjectAsync<List<MyMatchStats>>("MyMatchStats");
            return observableRes;
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