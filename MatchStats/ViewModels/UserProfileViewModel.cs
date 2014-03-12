using System;
using System.Runtime.Serialization;
using Akavache;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class UserProfileViewModel : ReactiveObject
    {
        public IReactiveCommand NavAwayCommand { get; protected set; }

        public UserProfileViewModel()
        {
            NavAwayCommand = new ReactiveCommand();
            NavAwayCommand.Subscribe(_ => CloseSelf());

            RxApp.MutableResolver.GetService<IBlobCache>("UserAccount").GetObjectAsync<Player>("DefaultUser")
                .Subscribe(x => DefaultPlayer = x, ex => this.Log().ErrorException("Error getting DefaultPlayer", ex));

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
