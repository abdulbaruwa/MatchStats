﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class MatchScoreViewModel : ReactiveObject, IRoutableViewModel
    {
        public ReactiveList<IGameAction> ScorePoints { get; protected set; }
        public ReactiveList<IGameAction> PlayerOneActions { get; protected set; }
        public ReactiveList<IGameAction> PlayerTwoActions { get; protected set; }
        public IReactiveCommand NavToHomePageCommand { get; protected set; }
        public IReactiveCommand StartMatchCommand { get; protected set; }
        private readonly IReactiveCommand addItemsCommand;

        public MatchScoreViewModel(IScreen screen = null)
        {
            PlayerOneActions = new ReactiveList<IGameAction>();
            PlayerTwoActions = new ReactiveList<IGameAction>();
            ScorePoints = new ReactiveList<IGameAction>();
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MatchScore";
            NavToHomePageCommand = new ReactiveCommand();
            NavToHomePageCommand.Subscribe(_ => NavigateBackToHomePage());
            StartMatchCommand = new ReactiveCommand();
            StartMatchCommand.Subscribe(StartMatch);
            NewMatchControlViewModel = RxApp.DependencyResolver.GetService<NewMatchControlViewModel>();
            MessageBus.Current.Listen<Match>().InvokeCommand(StartMatchCommand);
            RandomGuid = Guid.NewGuid();

            //Observe the NewMatchControlVM.ShowMe property, Hide pop up depending on value.
            _showHidePop = this.WhenAny(x => x.NewMatchControlViewModel.ShowMe, x => x.Value)
                .Where(x => x == false)
                .Select(x => x)
                .ToProperty(this, x => x.ShowHidePopup, true);

            //Observe the NewMatchControlVM.ShowMe property, if just set call start match and set the CurrentMatch Property
            _currentMatch = this.WhenAny(x => x.NewMatchControlViewModel.ShowMe, x => x.Value)
                .Where(x => x == false)
                .Select(x => this.NewMatchControlViewModel.SavedMatch)
                .Do(StartMatch)
                .ToProperty(this, x => x.CurrentMatch, new Match());
     }

        private  IObservable<IGameAction> GetGameCommandsForPlayer(Player player)
        {
            var listOfActions = new List<IGameAction>
            {
                new DoubleFaultCommand(player),
                new ForeHandWinnerCommand(player),
            };

            return listOfActions.ToObservable();
        }

        private void StartMatch(object param)
        {
            var match = param as Match;
            if (match != null)
            {
                var matchStatsApi = RxApp.DependencyResolver.GetService<IMatchStatsApi>();
                matchStatsApi.SaveMatch(match);
                PlayerOneActions.Clear();

                GetGameCommandsForPlayer(match.PlayerOne).Subscribe(x => PlayerOneActions.Add(x));
                GetGameCommandsForPlayer(match.PlayerTwo).Subscribe(x => PlayerTwoActions.Add(x));
            }
        }

        private void NavigateBackToHomePage()
        {
            HostScreen.Router.NavigateBack.Execute(null);
        }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid
        {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }
        
        private ObservableAsPropertyHelper<Match> _currentMatch;
        public Match CurrentMatch
        {
            get { return _currentMatch.Value; }
        }

        private ObservableAsPropertyHelper<List<IGameAction>> _playerOneActions;
        public List<IGameAction> PlayerOneCommands
        {
            get { return _playerOneActions.Value; }
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

        ObservableAsPropertyHelper<string> _playerOnesName;
        public string PlayerOnesName
        {
            get { return _playerOnesName.Value; }
        }

        ObservableAsPropertyHelper<bool> _showHidePop;
        public bool ShowHidePopup
        { get { return _showHidePop.Value; } }
            
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