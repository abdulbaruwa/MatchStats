using System;
using System.Runtime.Serialization;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class MatchesPlayedViewModel : ReactiveObject, IRoutableViewModel
    {
        private Player _defaultPlayer;
        private ReactiveList<MyMatchStats> _myMatchStats;
        private NewMatchControlViewModel _newMatchControlViewModel;
        private bool _showNewMatchPopup;

        public MatchesPlayedViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MyMatchStats";

            // AddMatch Command is only fired when Popup is not being displayed
            AddMatch = new ReactiveCommand(this.WhenAny(vm => vm.ShowNewMatchPopup, s => ! s.Value));
            AddMatch.Subscribe(_ => ShowOrAddMatchPopUp());

            StartMatch = new ReactiveCommand(this.WhenAny(vm => vm.ShowNewMatchPopup, s => s.Value));
            StartMatch.Subscribe(_ => NavigagteToMatchScoreViewModel());

            //Register for Save message from NewMatchControlViewModel
            MessageBus.Current.Listen<NewMatchControlViewModel>().InvokeCommand(StartMatch);
        }

        public ReactiveList<MyMatchStats> MyMatchStats
        {
            get { return _myMatchStats; }
            set { this.RaiseAndSetIfChanged(ref _myMatchStats, value); }
        }

        public Player DefaultPlayer
        {
            get { return _defaultPlayer; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayer, value); }
        }

        public bool ShowNewMatchPopup
        {
            get { return _showNewMatchPopup; }
            set { this.RaiseAndSetIfChanged(ref _showNewMatchPopup, value); }
        }

        public NewMatchControlViewModel NewMatchControlViewModel
        {
            get { return _newMatchControlViewModel; }
            set { this.RaiseAndSetIfChanged(ref _newMatchControlViewModel, value); }
        }

        public IReactiveCommand AddMatch { get; protected set; }
        public IReactiveCommand StartMatch { get; protected set; }
        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }

        private void NavigagteToMatchScoreViewModel()
        {
            // Use the viewmodel from the popup
            HideAddMatchPopUp(null);
            HostScreen.Router.Navigate.Execute(new MatchScoreViewModel(HostScreen));
        }

        private void ShowOrAddMatchPopUp()
        {
            ShowNewMatchPopup = true;
        }

        private void HideAddMatchPopUp(object o)
        {
            ShowNewMatchPopup = false;
        }
    }
}