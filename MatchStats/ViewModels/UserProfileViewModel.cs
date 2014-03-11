using System;
using System.Runtime.Serialization;
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
