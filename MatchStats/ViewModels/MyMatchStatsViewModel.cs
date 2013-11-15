﻿using System.Runtime.Serialization;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
 

    [DataContract]
    public class MyMatchStatsViewModel : ReactiveObject, IRoutableViewModel
    {
        public MyMatchStatsViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MyMatchStats";
        }

        [DataMember]
        RoutingState _Router;

        public IRoutingState Router
        {
            get { return _Router; }
            set { _Router = (RoutingState)value; }
        }

        private void ShowOrAddMatchPopUp()
        {
            ShowNewMatchPopup = true;
        }
        private void HideAddMatchPopUp()
        {
            ShowNewMatchPopup = false;
            StartMatch.Execute(null);
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

        public ReactiveCommand AddMatch { get; protected set; }
        public IReactiveCommand StartMatch { get; protected set; }
        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }
    }
}