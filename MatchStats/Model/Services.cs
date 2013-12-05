using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace MatchStats.Model
{
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