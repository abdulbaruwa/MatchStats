using System;
using System.Runtime.Serialization;
using Akavache;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public interface IMatchesPlayedViewModel : IRoutableViewModel
    {
        IReactiveCommand AddMatch { get; set; }

        ReactiveList<MyMatchStats> MyMatchStats { get; set; }
        NewMatchControlViewModel NewMatchControlViewModel { get; set; }
        bool CredentialAuthenticated { get; set; }
        Player DefaultPlayer { get; set; }
        bool ShowNewMatchPopup { get; set; }
    }

    [DataContract]
    public class MatchesPlayedViewModel : ReactiveObject, IMatchesPlayedViewModel
    {
        public MatchesPlayedViewModel(ILoginMethods loginMethods, IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MyMatchStats";

            // AddMatch Command is only fired when Popup is not being displayed
            AddMatch = new ReactiveCommand(this.WhenAny(vm => vm.ShowNewMatchPopup, s => ! s.Value));
            AddMatch.Subscribe(_ => ShowOrAddMatchPopUp());

            loginMethods.SaveCredentials(Token);
            RxApp.MutableResolver.GetService<ISecureBlobCache>().GetObjectAsync<string>("Token")
                .Subscribe(x => CredentialAuthenticated = true);

            MyMatchStats = new ReactiveList<MyMatchStats>();
            var matchStatsApi = RxApp.MutableResolver.GetService<IMatchStatsApi>();

            var obser = matchStatsApi.FetchMatchStats();
            obser.Subscribe(x =>
            {
                if (x != null)
                {
                    MyMatchStats.AddRange(x);
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
        private ReactiveList<MyMatchStats> _myMatchStats;
        public ReactiveList<MyMatchStats> MyMatchStats
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

        private NewMatchControlViewModel _newMatchControlViewModel;
        public NewMatchControlViewModel NewMatchControlViewModel
        {
            get { return _newMatchControlViewModel; }
            set { this.RaiseAndSetIfChanged(ref _newMatchControlViewModel, value); }
        }

        public IReactiveCommand AddMatch { get; set; }
        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }
        
        private void ShowOrAddMatchPopUp()
        {
            HostScreen.Router.Navigate.Execute(RxApp.DependencyResolver.GetService<MatchScoreViewModel>());// new MatchScoreViewModel(HostScreen));
        }
    }
}