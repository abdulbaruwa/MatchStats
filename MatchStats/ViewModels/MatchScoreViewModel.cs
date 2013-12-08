using System;
using System.Runtime.Serialization;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class MatchScoreViewModel : ReactiveObject, IRoutableViewModel
    {
        public ReactiveList<ScorePoint> ScorePoints { get; protected set; }
        public IReactiveCommand NavToHomePageCommand { get; protected set; }
        public IReactiveCommand StartMatchCommand { get; protected set; }

        public MatchScoreViewModel(IScreen screen = null)
        {
            ScorePoints = new ReactiveList<ScorePoint>();
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MatchScore";
            NavToHomePageCommand = new ReactiveCommand();
            NavToHomePageCommand.Subscribe(_ => NavigateBackToHomePage());
            StartMatchCommand = new ReactiveCommand();
            StartMatchCommand.Subscribe(StartMatch);

            MessageBus.Current.Listen<NewMatchControlViewModel>().InvokeCommand(StartMatchCommand);
        }

        private void StartMatch(object param)
        {
            ShowHideMatchPopup = false;
        }

        private void NavigateBackToHomePage()
        {
            HostScreen.Router.NavigateBack.Execute(null);
        }

        [DataMember]
        private string _playerOneCurrentGame = "";
        public string PlayerOneCurrentGame
        {
            get { return _playerOneCurrentGame; }
            set { this.RaiseAndSetIfChanged(ref _playerOneCurrentGame, value); }
        }

        [DataMember]
        private string _playerTwoCurrentGame = "";
        public string PlayerTwoCurrentGame
        {
            get { return _playerTwoCurrentGame; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoCurrentGame, value); }
        }

        [DataMember]
        private string _playerOnesName = "";
        public string PlayerOnesName
        {
            get { return _playerOnesName; }
            set { this.RaiseAndSetIfChanged(ref _playerOnesName, value); }
        }

        [DataMember]
        private string _playerTwosName = "";
        public string PlayerTwosName
        {
            get { return _playerTwosName; }
            set { this.RaiseAndSetIfChanged(ref _playerTwosName, value); }
        }

        [DataMember]
        private string _playerTwoThirdSet = "";
        public string PlayerTwoThirdSet
        {
            get { return _playerTwoThirdSet; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoThirdSet, value); }
        }

        [DataMember]
        private string _playerOneThirdSet = "";
        public string PlayerOneThirdSet
        {
            get { return _playerOneThirdSet; }
            set { this.RaiseAndSetIfChanged(ref _playerOneThirdSet, value); }
        }

        [DataMember]
        private string _playerOneSecondSet = "";
        public string PlayerOneSecondSet
        {
            get { return _playerOneSecondSet; }
            set { this.RaiseAndSetIfChanged(ref _playerOneSecondSet, value); }
        }

        [DataMember]
        private string _playerTwoSecondSet = "";
        public string PlayerTwoSecondSet
        {
            get { return _playerTwoSecondSet; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoSecondSet, value); }
        }

        [DataMember]
        private string _playerOneFirstSet = "";
        public string PlayerOneFirstSet
        {
            get { return _playerOneFirstSet; }
            set { this.RaiseAndSetIfChanged(ref _playerOneFirstSet, value); }
        }

        [DataMember]
        private string _playerTwoFirstSet = "";
        public string PlayerTwoFirstSet
        {
            get { return _playerTwoFirstSet; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoFirstSet, value); }
        }

        [DataMember]
        private bool _showHideMatchPopup;
        public bool ShowHideMatchPopup
        {
            get { return _showHideMatchPopup; }
            set { this.RaiseAndSetIfChanged(ref _showHideMatchPopup, value); }
        }

        [DataMember]
        private NewMatchControlViewModel _newMatchControlViewModel;
        public NewMatchControlViewModel NewMatchControlViewModel
        {
            get { return _newMatchControlViewModel; }
            set { this.RaiseAndSetIfChanged(ref _newMatchControlViewModel, value); }
        }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }
    }

    
}