using System;
using System.Threading.Tasks;
using Akavache;
using ReactiveUI;
using MatchStats.Model;
using System.Runtime.Serialization;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class UserProfileViewModel : ReactiveObject
    {
        public IReactiveCommand SaveDefaultPlayerCommand { get; protected set; }
        public IReactiveCommand NavAwayCommand { get; protected set; }
        private IBlobCache _blobCache;
        public UserProfileViewModel()
        {
            SaveDefaultPlayerCommand = new ReactiveCommand(this.WhenAny(x => x.DefaultPlayer, p => PlayerIsValidForSave(p.Value)));
            SaveDefaultPlayerCommand.Subscribe(_ => SaveDefaultPlayer());
            NavAwayCommand = new ReactiveCommand();
            NavAwayCommand.Subscribe(_ => CloseSelf());
            _blobCache = RxApp.MutableResolver.GetService<IBlobCache>("UserAccount");
            _blobCache.GetObjectAsync<Player>("DefaultPlayer")
                .Subscribe(x => DefaultPlayer = x, ex => this.Log().ErrorException("Error getting DefaultPlayer", ex));
        }

        private bool PlayerIsValidForSave(Player player)
        {
            if (player == null) return false;
            return  !  (string.IsNullOrEmpty(player.FirstName) || string.IsNullOrEmpty(player.SurName) || string.IsNullOrEmpty(player.Rating));
        }

        private void SaveDefaultPlayer()
        {
            _blobCache.InsertObject("DefaultPlayer", DefaultPlayer);
        }

        [DataMember]
        private Player _defaultPlayer;
        public Player DefaultPlayer
        {
            get { return _defaultPlayer; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayer, value); }
        }

        private void CloseSelf()
        {
            ShowMe = false;
        }

        [DataMember]
        private bool _showMe;
        public bool ShowMe
        {
            get { return _showMe; }
            set { this.RaiseAndSetIfChanged(ref _showMe, value); }
        }
    }
}
