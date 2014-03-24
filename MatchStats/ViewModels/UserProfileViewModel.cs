using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Akavache;
using MatchStats.Common;
using MatchStats.Enums;
using ReactiveUI;
using MatchStats.Model;
using System.Runtime.Serialization;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class UserProfileViewModel : ReactiveObject
    {
        public IReactiveCommand UpdateProfileCommand { get; protected set; }
        public IReactiveCommand NavAwayCommand { get; protected set; }
        public IReactiveCommand BrowseImageCommand { get; protected set; }
        private IBlobCache _blobCache;
        public UserProfileViewModel()
        {
            UpdateProfileCommand = new ReactiveCommand(this.WhenAny(x => x.PlayerFirstName, x => x.PlayerSurname, x => x.SelectedPlayerRating, (firstname, surname, rating) => PlayerIsValidForSave(firstname.Value, surname.Value, rating.Value)));
            UpdateProfileCommand.Subscribe(_ => SaveDefaultPlayer());
            NavAwayCommand = new ReactiveCommand();
            NavAwayCommand.Subscribe(_ => CloseSelf());
            BrowseImageCommand = new ReactiveCommand();
            BrowseImageCommand.Subscribe((_ => BrowseImage()));
            _blobCache = RxApp.MutableResolver.GetService<IBlobCache>("UserAccount");
            _blobCache.GetObjectAsync<Player>("DefaultPlayer")
                .Subscribe(x => DefaultPlayer = x, ex => this.Log().ErrorException("Error getting DefaultPlayer", ex));
            this.WhenAny(x => x.DefaultPlayer, x => x.Value).Where(x => x != null).Select(x => x).Subscribe(UpdateProfileDisplay);
            this.WhenAny(x => x.DefaultPlayerImagePath, x => x.Value).Where(x => x != null).Select(x => x)
                .Subscribe(x =>
                {
                    //RxApp.MutableResolver.GetService<ImagesApi>().BrowseImage()
                    var baseUri = new Uri("ms-appx:///");
                    //DefaultPlayerImage = new BitmapImage(new Uri(baseUri, DefaultPlayerImagePath));
                });
            GetDefaultProfileImage();
        }

        private async void GetDefaultProfileImage()
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("DefaultPlayerImage.bmp") != null)
            {
                var defaultPlayerImage = await ApplicationData.Current.LocalFolder.GetFileAsync("DefaultPlayerImage.bmp") ;
                var imageThumbnail = defaultPlayerImage.GetThumbnailAsync(ThumbnailMode.PicturesView);
                SetDefaultImage(await imageThumbnail);
            }
        }

        private async void BrowseImage()
        {
            var imageApi = RxApp.MutableResolver.GetService<IImagesApi>();
            var imagePath = await imageApi.BrowseImageThumbnail();
            if (imagePath != null)
            {
                SetDefaultImage(imagePath);
            }
        }

        private void SetDefaultImage(StorageItemThumbnail imagePath)
        {
            var image = new BitmapImage();
            image.SetSource(imagePath);
            DefaultPlayerImage = image;
        }

        private void UpdateProfileDisplay(Player player)
        {
            PlayerFirstName = player.FirstName;
            PlayerSurname = player.SurName;
            SelectedPlayerRating = player.Rating;
        }

        private bool PlayerIsValidForSave(string firstname, string surname, object rating)
        {
            return !string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(surname) && rating != null;
        }

        private void SaveDefaultPlayer()
        {
            DefaultPlayer = new Player()
            {
                FirstName = PlayerFirstName,
                SurName = PlayerSurname,
                Rating = SelectedPlayerRating.ToString()
            };
            _blobCache.InsertObject("DefaultPlayer", DefaultPlayer);
        }

        [DataMember]
        private Player _defaultPlayer;
        public Player DefaultPlayer
        {
            get { return _defaultPlayer; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayer, value); }
        }

        [DataMember]
        private bool _showMe;
        public bool ShowMe
        {
            get { return _showMe; }
            set { this.RaiseAndSetIfChanged(ref _showMe, value); }
        }

        private static IEnumerable<NameAndDescription> _ratingsDictionary;
        public IEnumerable<NameAndDescription> RatingsDictionary
        {
            get { return _ratingsDictionary ?? (_ratingsDictionary = EnumHelper.GetEnumKeyValuePairs<Rating>()); }
        }

        private static List<Rating> _ratings;
        public List<Rating> Ratings
        {
            get { return _ratings ?? (_ratings = EnumHelper.GetEnumAsList<Rating>()); }
        }

        [DataMember]
        private object _selectedPlayerRating = string.Empty;
        public object SelectedPlayerRating
        {
            get { return _selectedPlayerRating; }
            set
            {
                if(value != null)this.RaiseAndSetIfChanged(ref _selectedPlayerRating, value);
            }
        }

        [DataMember]
        private string _playerFirstName = string.Empty;
        public string PlayerFirstName
        {
            get { return _playerFirstName; }
            set { this.RaiseAndSetIfChanged(ref _playerFirstName, value); }
        }

        [DataMember]
        private string _playerSurname = string.Empty;
        public string PlayerSurname
        {
            get { return _playerSurname; }
            set { this.RaiseAndSetIfChanged(ref _playerSurname, value); }
        }

        [DataMember]
        private object _selectedPlayerRatingValue;
        public object SelectedPlayerRatingValue
        {
            get { return _selectedPlayerRatingValue; }
            set
            {
                if (value != null) this.RaiseAndSetIfChanged(ref _selectedPlayerRatingValue, value);
            }
        }

        [DataMember]
        private string _defaultPlayerImagePath;

        public string DefaultPlayerImagePath
        {
            get { return _defaultPlayerImagePath; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayerImagePath, value); }
        }

        [DataMember]
        private ImageSource _defaultPlayerImage;
        public ImageSource DefaultPlayerImage
        {
            get { return _defaultPlayerImage; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayerImage, value);}
        }

        private void CloseSelf()
        {
            ShowMe = false;
        }
    }
}
