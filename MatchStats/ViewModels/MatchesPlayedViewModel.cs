using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Akavache;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public interface IMatchesPlayedViewModel : IRoutableViewModel
    {
        IReactiveCommand AddMatch { get; set; }
        ReactiveList<Match> MyMatchStats { get; set; }
        NewMatchControlViewModel NewMatchControlViewModel { get; set; }
        UserProfileViewModel UserProfileViewModel { get; set; }
        bool CredentialAuthenticated { get; set; }
        Player DefaultPlayer { get; set; }
        bool ShowNewMatchPopup { get; set; }
        IReactiveCommand FetchLatestMatchesPlayed { get; set; }
    }

    [DataContract]
    public class MatchesPlayedViewModel : ReactiveObject, IMatchesPlayedViewModel
    {
        public MatchesPlayedViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MyMatchStats";
        }

        public MatchesPlayedViewModel(ILoginMethods loginMethods, IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MyMatchStats";

            // AddMatch Command is only fired when Popup is not being displayed
            AddMatch = new ReactiveCommand(this.WhenAny(vm => vm.ShowNewMatchPopup, s => ! s.Value));
            AddMatch.Subscribe(_ => ShowOrAddMatchPopUp());

            EditProfile = new ReactiveCommand();
            EditProfile.Subscribe(_ => ShowProfilePopUp());

            FetchLatestMatchesPlayed = new ReactiveCommand();
            FetchLatestMatchesPlayed.Subscribe(_ => FetchLatestMatchStats());

            loginMethods.SaveCredentials(Token);
            RxApp.MutableResolver.GetService<ISecureBlobCache>().GetObjectAsync<string>("Token")
                .Subscribe(x => CredentialAuthenticated = true);

            this.WhenAny(vm => vm.SelectedMatchStat, x => x.Value)
                .Where(x => x != null && x is Match)
                .Select(x => (Match)x).Subscribe(ShowMatchStatForMatch);

            this.WhenAny(x => x.UserProfileViewModel.ShowMe, x => x.Value)
                .Where(x => x == false)
                .Select(x => x)
                .Subscribe(x => this.ShowHideProfilePopup = x);

            //AMatchIsOver
            MessageBus.Current.Listen<string>("MatchOver").Subscribe(_ => FetchLatestMatchStats());
            MyMatchStats = new ReactiveList<Match>();
            FetchLatestMatchStats();
        }

        private void FetchLatestMatchStats()
        {
            MyMatchStats.Clear();
            var matchStatsApi = RxApp.MutableResolver.GetService<IMatchStatsApi>();

            var obser = matchStatsApi.FetchMatchStats();
            obser.Subscribe(x =>
            {
                if (x != null)
                {
                    var matchsPlayed = x.Where(y => y.IsMatchOver).ToList();
                    if (matchsPlayed.Count > 0) MyMatchStats.AddRange(matchsPlayed);
                }
            },
                ex =>
                {
                    //It may be the first time we are running the App so Log and move on.
                    this.Log().Info("No Match Stats available for user");
                });
        }

        [DataMember]
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { this.RaiseAndSetIfChanged(ref _userName, value); }
        }
        
        [DataMember]
        private ReactiveList<Match> _myMatchStats;
        public ReactiveList<Match> MyMatchStats
        {
            get { return _myMatchStats; }
            set { this.RaiseAndSetIfChanged(ref _myMatchStats, value); }
        }

        [DataMember]
        private bool _credentialsAuthenticated;
        public bool CredentialAuthenticated
        {
            get { return _credentialsAuthenticated; }
            set { this.RaiseAndSetIfChanged(ref _credentialsAuthenticated, value); }
        }

        [DataMember]
        private string _token;
        public string Token
        {
            get { return _token; }
            set { this.RaiseAndSetIfChanged(ref _token, value); }
        }

        [DataMember]
        private Player _defaultPlayer;
        public Player DefaultPlayer
        {
            get { return _defaultPlayer; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayer, value); }
        }

        [DataMember]
        private bool _showNewMatchPopup;
        public bool ShowNewMatchPopup
        {
            get { return _showNewMatchPopup; }
            set { this.RaiseAndSetIfChanged(ref _showNewMatchPopup, value); }
        }

        public IReactiveCommand FetchLatestMatchesPlayed { get; set; }

        [DataMember]
        private object _selectedMatchStat;
        public object SelectedMatchStat
        {
            get { return _selectedMatchStat; }
            set { this.RaiseAndSetIfChanged(ref _selectedMatchStat, value); }
        }

        private bool _showHideProfilePopup;
        public bool ShowHideProfilePopup
        {
            get { return _showHideProfilePopup; }
            set { this.RaiseAndSetIfChanged(ref _showHideProfilePopup, value); }
        }

        private NewMatchControlViewModel _newMatchControlViewModel;
        public NewMatchControlViewModel NewMatchControlViewModel
        {
            get { return _newMatchControlViewModel; }
            set { this.RaiseAndSetIfChanged(ref _newMatchControlViewModel, value); }
        }

        [DataMember]
        private UserProfileViewModel _userProfileViewModel;
        public UserProfileViewModel  UserProfileViewModel
        {
            get { return _userProfileViewModel; }
            set { this.RaiseAndSetIfChanged(ref _userProfileViewModel, value); }
        }

        public IReactiveCommand AddMatch { get; set; }
        public IReactiveCommand EditProfile { get; set; }
        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }
        
        private void ShowProfilePopUp()
        {
            var userProfileV  = RxApp.DependencyResolver.GetService<UserProfileViewModel>();
            UserProfileViewModel = userProfileV;
            userProfileV.ShowMe = true;
        }

        private void ShowOrAddMatchPopUp()
        {
            var matchScoreVm = RxApp.DependencyResolver.GetService<MatchScoreViewModel>();
            matchScoreVm.ShowHideMatchPopup = true;

            HostScreen.Router.Navigate.Execute(matchScoreVm);
        }

        private void ShowMatchStatForMatch(Match matchStat)
        {
            var matchStatsViewModel = RxApp.DependencyResolver.GetService<MatchStatsViewModel>();
            matchStatsViewModel.CurrentMatch = matchStat;
            HostScreen.Router.Navigate.Execute(matchStatsViewModel);
        }
    }
}
