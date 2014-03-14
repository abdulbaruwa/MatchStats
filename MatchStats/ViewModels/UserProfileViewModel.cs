using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
        private IBlobCache _blobCache;
        public UserProfileViewModel()
        {
            UpdateProfileCommand = new ReactiveCommand(this.WhenAny(x => x.PlayerFirstName, x => x.PlayerSurname, x => x.SelectedPlayerRating, (firstname, surname, rating) => PlayerIsValidForSave(firstname.Value, surname.Value, rating.Value)));
            UpdateProfileCommand.Subscribe(_ => SaveDefaultPlayer());
            NavAwayCommand = new ReactiveCommand();
            NavAwayCommand.Subscribe(_ => CloseSelf());
            _blobCache = RxApp.MutableResolver.GetService<IBlobCache>("UserAccount");
            _blobCache.GetObjectAsync<Player>("DefaultPlayer")
                .Subscribe(x => DefaultPlayer = x, ex => this.Log().ErrorException("Error getting DefaultPlayer", ex));
            this.WhenAny(x => x.DefaultPlayer, x => x.Value).Where(x => x != null).Select(x => x).Subscribe(UpdateProfileDisplay);
        }

        private void UpdateProfileDisplay(Player player)
        {
            PlayerFirstName = player.FirstName;
            PlayerSurname = player.SurName;
            SelectedPlayerRating = (object)player.Rating;
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

        private static List<Rating> _ratings;
        public List<Rating> Ratings
        {
            get { return _ratings ?? (_ratings = EnumHelper.GetEnumAsList<Rating>()); }
        }

        [DataMember]
        private object _selectedPlayerRating;
        public object SelectedPlayerRating
        {
            get { return _selectedPlayerRating; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlayerRating, value); }
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

        private void CloseSelf()
        {
            ShowMe = false;
        }

    }
}
