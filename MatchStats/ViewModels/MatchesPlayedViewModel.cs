﻿using System;
using System.Runtime.Serialization;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    public interface IMatchesPlayedViewModel : IRoutableViewModel
    {
        IReactiveCommand AddMatch { get; set; }

        ReactiveList<MyMatchStats> MyMatchStats { get; set; }
        NewMatchControlViewModel NewMatchControlViewModel { get; set; }
        Player DefaultPlayer { get; set; }
        bool ShowNewMatchPopup { get; set; }
    }

    [DataContract]
    public class MatchesPlayedViewModel : ReactiveObject, IMatchesPlayedViewModel
    {

        public MatchesPlayedViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MyMatchStats";

            // AddMatch Command is only fired when Popup is not being displayed
            AddMatch = new ReactiveCommand(this.WhenAny(vm => vm.ShowNewMatchPopup, s => ! s.Value));
            AddMatch.Subscribe(_ => ShowOrAddMatchPopUp());
        }

        private ReactiveList<MyMatchStats> _myMatchStats;
        public ReactiveList<MyMatchStats> MyMatchStats
        {
            get { return _myMatchStats; }
            set { this.RaiseAndSetIfChanged(ref _myMatchStats, value); }
        }

        private Player _defaultPlayer;
        public Player DefaultPlayer
        {
            get { return _defaultPlayer; }
            set { this.RaiseAndSetIfChanged(ref _defaultPlayer, value); }
        }

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
            HostScreen.Router.Navigate.Execute(new MatchScoreViewModel(HostScreen));
        }
    }
}