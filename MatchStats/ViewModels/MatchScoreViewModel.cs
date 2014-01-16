using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using MatchStats.Common;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class MatchScoreViewModel : ReactiveObject, IRoutableViewModel
    {
        public ReactiveList<IGameActionViewModel> ScorePoints { get; protected set; }
        public ReactiveList<IGameActionViewModel> PlayerOneActions { get; protected set; }
        public ReactiveList<IGameActionViewModel> PlayerTwoActions { get; protected set; }
        public IReactiveCommand NavToHomePageCommand { get; protected set; }
        public IReactiveCommand StartMatchCommand { get; protected set; }
        public IReactiveCommand PlayerOneActionCommand { get; protected set; }
        public IReactiveCommand SetPlayerOneAsCurrentServerCommand { get; protected set; }
        public IReactiveCommand SetPlayerTwoAsCurrentServerCommand { get; protected set; }
        public IReactiveCommand PlayerOneFirstServeInCommand { get; protected set; }
        public IReactiveCommand PlayerOneFirstServeOutCommand { get; protected set; }
        public IReactiveCommand PlayerOneSecondServeInCommand { get; protected set; }
        public IReactiveCommand PlayerTwoFirstServeInCommand { get; protected set; }
        public IReactiveCommand PlayerTwoFirstServeOutCommand { get; protected set; }
        public IReactiveCommand PlayerTwoSecondServeInCommand { get; protected set; }
        public IReactiveCommand UndoLastActionCommand { get; protected set; }

        private readonly IReactiveCommand addItemsCommand;

        public MatchScoreViewModel(IScreen screen = null)
        {
            InitializeServeCommands();

            PlayerOneActions = new ReactiveList<IGameActionViewModel>();
            PlayerTwoActions = new ReactiveList<IGameActionViewModel>();

            RandomGuid = Guid.NewGuid();
            UrlPathSegment = "MatchScore";
            
            ScorePoints = new ReactiveList<IGameActionViewModel>();
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            NavToHomePageCommand = new ReactiveCommand();
            NavToHomePageCommand.Subscribe(_ => NavigateBackToHomePage());
            
            StartMatchCommand = new ReactiveCommand();
            StartMatchCommand.Subscribe(StartMatch);
            
            NewMatchControlViewModel = RxApp.DependencyResolver.GetService<NewMatchControlViewModel>();
            
            InitializeCurrentServerCommands();
            
            WhenAnyPropertyBindings();

            ActionCommandsEnableBindings();

            MessageBus.Current.Listen<Match>("PointUpdateForCurrentMatch").Subscribe(x =>
            {
                CurrMatch = x;
                ToggleActionsOffForBothPlayers();

            });

            MessageBus.Current.Listen<Match>("AceServeForCurrentMatch").Subscribe(x =>
            {
                //Deal with match over scenario
                if (CurrentServer.IsPlayerOne)
                {
                    EnableAceServeForPlayerOne();
                }
                else
                {
                    EnableAceServeForPlayerTwo();
                }
            });

            MessageBus.Current.Listen<Match>("NonPointUpdateForCurrentMatch").Subscribe(x =>
            {
                CurrMatch = x;
            });
        }

        private void ToggleActionsOffForBothPlayers()
        {
            foreach (var action in PlayerOneActions.Where(x => x.Name != "DoubleFault" && x.Name != "AceServe"))
            {
                action.IsEnabled = false;
            }

            foreach (var action in PlayerTwoActions.Where(x => x.Name != "DoubleFault" && x.Name != "AceServe"))
            {
                action.IsEnabled = false;
            }

            if (CurrentServer != null && CurrentServer.IsPlayerOne)
            {
                EnableAceServeForPlayerOne();
            }

            if (CurrentServer != null && ! CurrentServer.IsPlayerOne)
            {
                EnableAceServeForPlayerTwo();
            }
        }

        private void EnableAceServeForPlayerOne()
        {
            PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled = true;
            PlayerTwoActions.First(x => x.Name == "AceServe").IsEnabled = false;
        }

        private void EnableAceServeForPlayerTwo()
        {
            PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled = false;
            PlayerTwoActions.First(x => x.Name == "AceServe").IsEnabled = true;
        }

        private void ActionCommandsEnableBindings()
        {
            this.WhenAny(x => x.CurrentServer, x => x.CurrMatch.Score.IsMatchOver,
                (server, isMatchOver) => server.Value != null && isMatchOver.Value == false)
                .Subscribe(x =>
                {
                    var lastStat = CurrMatch.MatchStats.LastOrDefault();
                    if (CurrentServer != null && CurrentServer.IsPlayerOne)
                    {
                        if (lastStat != null && lastStat.Reason == StatDescription.FirstServeOut)
                        {
                            PlayerOneActions.First(y => y.Name == "DoubleFault").IsEnabled = true;
                            PlayerTwoActions.First(y => y.Name == "DoubleFault").IsEnabled = false;
                            EnableAceServeForPlayerOne();
                        }
                    }

                    if (CurrentServer != null && (!CurrentServer.IsPlayerOne))
                    {
                        if (lastStat != null && lastStat.Reason == StatDescription.FirstServeOut)
                        {
                            PlayerTwoActions.First(y => y.Name == "DoubleFault").IsEnabled = true;
                            PlayerOneActions.First(y => y.Name == "DoubleFault").IsEnabled = false;
                            EnableAceServeForPlayerTwo();
                        } 
                    }
                });

            // Disable the Double Fault Action immediately after a double fault for Player One
            this.WhenAny(x => x.CurrMatch.MatchStats, x => x.Value.LastOrDefault())
                .Where(x => x != null && x.Server.IsPlayerOne && x.Reason == StatDescription.DoubleFault)
                .Subscribe(_ =>
                {
                    PlayerOneActions.First(x => x.Name == "DoubleFault").IsEnabled = false;
                });

            // Disable the Double Fault Action immediately after a double fault Player Two
            this.WhenAny(x => x.CurrMatch.MatchStats, x => x.Value.LastOrDefault())
                .Where(x => x != null && (! x.Server.IsPlayerOne) && x.Reason == StatDescription.DoubleFault)
                .Subscribe(_ =>
                {
                    PlayerTwoActions.First(x => x.Name == "DoubleFault").IsEnabled = false;
                });

            // Enable Other Actions once a serve is in
            this.WhenAny(x => x.CurrMatch.MatchStats, x => x.Value.LastOrDefault())
                .Where(x => x != null && (x.Reason == StatDescription.FirstServeIn || x.Reason == StatDescription.SecondServeIn))
                .Subscribe(_ =>
                {
                    foreach (var action in PlayerOneActions.Where(x => x.Name != "DoubleFault" && x.Name != "AceServe").Concat(PlayerTwoActions.Where(x => x.Name != "DoubleFault" && x.Name != "AceServe")))
                    {
                        action.IsEnabled = true;
                    }

                    foreach (
                        var action in
                            PlayerOneActions.Where(x => x.Name == "DoubleFault" && x.Name != "AceServe")
                                .Concat(PlayerTwoActions.Where(x => x.Name == "DoubleFault" && x.Name != "AceServe")))
                    {
                        action.IsEnabled = false;
                    }
                });

            // If a serve is in Disable the ability to add An Ace Serve for player one
            this.WhenAny(x => x.CurrMatch.MatchStats, x => x.Value.LastOrDefault())
                .Where(x => x != null && x.Server.IsPlayerOne && (x.Reason == StatDescription.FirstServeIn || x.Reason == StatDescription.SecondServeIn))
                .Subscribe(_ =>
                {
                    PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled = false;
                });

            // If a serve is in Disable the ability to add An Ace Serve for player two
            this.WhenAny(x => x.CurrMatch.MatchStats, x => x.Value.LastOrDefault())
                .Where(x => x != null && (! x.Server.IsPlayerOne) && (x.Reason == StatDescription.FirstServeIn || x.Reason == StatDescription.SecondServeIn))
                .Subscribe(_ =>
                {
                    PlayerTwoActions.First(x => x.Name == "AceServe").IsEnabled = false;
                });
        }

        private void InitializeCurrentServerCommands()
        {
            SetPlayerOneAsCurrentServerCommand = new ReactiveCommand();
            SetPlayerOneAsCurrentServerCommand.Subscribe(_ =>
            {
                CurrMatch.Score.CurrentServer = CurrMatch.PlayerOne;
                CurrentServer = CurrMatch.PlayerOne;
                EnableAceServeForPlayerOne();
                PlayerOneIsServing = true;
                PlayerTwoIsServing = false;
                SaveMatch(CurrMatch);
            });

            SetPlayerTwoAsCurrentServerCommand = new ReactiveCommand();
            SetPlayerTwoAsCurrentServerCommand.Subscribe(_ =>
            {
                CurrMatch.Score.CurrentServer = CurrMatch.PlayerTwo;
                CurrentServer = CurrMatch.PlayerTwo;
                EnableAceServeForPlayerTwo();
                PlayerTwoIsServing = true;
                PlayerOneIsServing = false;
                SaveMatch(CurrMatch);
            });
        }

        private void WhenAnyPropertyBindings()
        {
            //Observe the NewMatchControlVM.ShowMe property, Hide pop up depending on value.
            _showHidePop = this.WhenAny(x => x.NewMatchControlViewModel.ShowMe, x => x.Value)
                .Where(x => x == false)
                .Select(x => x)
                .ToProperty(this, x => x.ShowHidePopup, true);

            //Observe the NewMatchControlVM.ShowMe property, if just set call start match and set the CurrentMatch Property
            this.WhenAny(x => x.NewMatchControlViewModel.ShowMe, x => x.Value)
                .Where(x => x == false)
                .Select(x => this.NewMatchControlViewModel.SavedMatch).Subscribe(x => StartMatch(x));

            _playerOneFirstSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.FirstOrDefault() != null)
                .Select(x =>
                        x.Sets.First()
                            .Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerOne.FullName)
                            .ToString())
                .ToProperty(this, x => x.PlayerOneFirstSet, "");

            _playerTwoFirstSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.FirstOrDefault() != null)
                .Select(x =>
                        x.Sets.First()
                            .Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerTwo.FullName)
                            .ToString())
                .ToProperty(this, x => x.PlayerTwoFirstSet, "");

            _playerOneSecondSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.SecondOrDefault() != null)
                .Select(x =>
                        x.Sets.SecondOrDefault()
                            .Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerOne.FullName)
                            .ToString())
                .ToProperty(this, x => x.PlayerOneSecondSet, "");

            _playerTwoSecondSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.SecondOrDefault() != null)
                .Select(x =>
                        x.Sets.SecondOrDefault()
                            .Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerTwo.FullName)
                            .ToString())
                .ToProperty(this, x => x.PlayerTwoSecondSet, "");

            _playerOneThirdSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.ThirdOrDefault() != null)
                .Select(x =>
                        x.Sets.ThirdOrDefault()
                            .Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerOne.FullName)
                            .ToString())
                .ToProperty(this, x => x.PlayerOneThirdSet, "");

            _playerTwoThirdSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.ThirdOrDefault() != null) //TODO: Need to 
                .Select(x =>
                        x.Sets.ThirdOrDefault()
                            .Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerTwo.FullName)
                            .ToString())
                .ToProperty(this, x => x.PlayerTwoThirdSet, "");

            _playerOneCurrentGame = this.WhenAny(x => x.CurrMatch, x => x.Value)
                .Where(x => x != null)
                .Select(x => x.GetPlayerOneCurrentScore())
                .ToProperty(this, x => x.PlayerOneCurrentGame, "");

            _playerTwoCurrentGame = this.WhenAny(x => x.CurrMatch, x => x.Value)
                .Where(x => x != null)
                .Select(x => x.GetPlayerTwoCurrentScore())
                .ToProperty(this, x => x.PlayerTwoCurrentGame, "");

            _matchStatus = this.WhenAny(x => x.CurrMatch.Score.Status, x => x.Value)
                .Select(x => x.GetAttribute<DisplayAttribute>().Name)
                .ToProperty(this, x => x.MatchStatus, "");


            this.WhenAny(x => x.CurrMatch.Score.CurrentServer, x => x.Value)
                .Where(x => x != null)
                .Select(x => x)
                .Subscribe(x =>
                {
                    CurrentServer = x;

                    if (x.IsPlayerOne)
                    {
                        PlayerTwoIsServing = false;
                        PlayerOneIsServing = true;
                    }
                    else
                    {
                        PlayerOneIsServing = false;
                        PlayerTwoIsServing = true;
                    }
                });
        }

        private void InitializeServeCommands()
        {
            PlayerOneFirstServeInCommand = new ReactiveCommand(FirstServePending(true));
            PlayerOneFirstServeOutCommand = new ReactiveCommand(FirstServePending(true));
            PlayerOneSecondServeInCommand = new ReactiveCommand(SecondServePending(true));

            PlayerOneFirstServeInCommand.Subscribe(_ => new FirstServeInCommandViewModel(null).ActionCommand.Execute(null));
            PlayerOneFirstServeOutCommand.Subscribe(_ => new FirstServeOutCommandViewModel(null).ActionCommand.Execute(null));
            PlayerOneSecondServeInCommand.Subscribe(_ => new SecondServeInCommandViewModel(null).ActionCommand.Execute(null));

            PlayerTwoFirstServeInCommand = new ReactiveCommand(FirstServePending(false));
            PlayerTwoFirstServeInCommand.Subscribe(
                _ => new FirstServeInCommandViewModel(CurrentServer).ActionCommand.Execute(null));

            PlayerTwoFirstServeOutCommand = new ReactiveCommand(FirstServePending(false));
            PlayerTwoFirstServeOutCommand.Subscribe(
                _ => new FirstServeOutCommandViewModel(CurrentServer).ActionCommand.Execute(null));

            PlayerTwoSecondServeInCommand = new ReactiveCommand(SecondServePending(false));
            PlayerTwoSecondServeInCommand.Subscribe(
                _ => new SecondServeInCommandViewModel(CurrentServer).ActionCommand.Execute(null));

            UndoLastActionCommand = new ReactiveCommand(CanUndoAction());
            UndoLastActionCommand.Subscribe(_ => new UndoLastActionCommandViewModel().Execute(null));
        }

        private  IObservable<IGameActionViewModel> GetGameCommandsForPlayer(Player player)
        {
            var listOfActions = new List<IGameActionViewModel>
            {
                new AceServeCommandViewModel(player),
                new DoubleFaultCommandViewModel(player),
                new ForeHandWinnerCommandViewModel(player),
            };

            return listOfActions.ToObservable();
        }

        private void StartMatch(object param)
        {
            var match = param as Match;
            if (match == null) return;
            CurrMatch = match;
            SaveMatch(match);
            PlayerOneActions.Clear();
                
            GetGameCommandsForPlayer(match.PlayerOne).Subscribe(x => PlayerOneActions.Add(x));
            GetGameCommandsForPlayer(match.PlayerTwo).Subscribe(x => PlayerTwoActions.Add(x));
        }

        private static void SaveMatch(Match match)
        {
            if (match == null) return;
            var matchStatsApi = RxApp.DependencyResolver.GetService<IMatchStatsApi>();
            matchStatsApi.SaveMatch(match);
        }

        private void NavigateBackToHomePage()
        {
            HostScreen.Router.NavigateBack.Execute(null);
        }

        private IObservable<bool> CanUndoAction()
        {
            return this.WhenAny(x => x.CurrMatch, x => ValidateForUndoCommand(x.Value));
        }

        private bool ValidateForUndoCommand(Match currentMatch)
        {
            if (currentMatch == null || currentMatch.Score.CurrentServer == null) return false;
            if (currentMatch.MatchStats.Count == 0) return false;
            return ! currentMatch.Score.IsMatchOver;
        }

        /// <summary>
        /// Rule for First Server in
        /// The last action is not a first serve in for player one with not further recorded point
        /// The Current Server is not player one
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private IObservable<bool> FirstServePending(bool isPlayerOne)
        {
            return this.WhenAny(x => x.CurrentServer, x => x.CurrMatch.MatchStats, (server, matchStats) => (
                        //The last action is not a first serve in for player with not further recorded point
                        ValidateForServeInOrOut(matchStats.Value.LastOrDefault(), server.Value, isPlayerOne)
                    ));
        }

        private bool ValidateForServeInOrOut(MatchStat matchStat, Player server,  bool isPlayerOne)
        {
            if (server == null) return false;
            if (server.IsPlayerOne == isPlayerOne)
            {
                if (matchStat == null) return true;
                if (server.IsPlayerOne == isPlayerOne &&
                    (matchStat.Reason == StatDescription.BreakPoint || matchStat.Reason == StatDescription.GamePoint ||
                     matchStat.Reason == StatDescription.GameOver))
                    return true;
                return server.IsPlayerOne == isPlayerOne && matchStat.PointWonLostOrNone != PointWonLostOrNone.NotAPoint;
            }
            return false;
        }

        private IObservable<bool> SecondServePending(bool isPlayerOne)
        {
            return this.WhenAny(x => x.CurrentServer, x => x.CurrMatch.MatchStats, (server, matchStats) => (
                ValidateForSecondServeCommand(matchStats.Value.LastOrDefault(), isPlayerOne))
                );
        }

        private bool ValidateForSecondServeCommand(MatchStat matchStat, bool isPlayerOne)
        {
            if (matchStat == null || matchStat.Server == null) return false;
            if (matchStat.Server.IsPlayerOne == isPlayerOne)
            {
                return matchStat.Reason == StatDescription.FirstServeOut;
            }
            return false;
        }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid
        {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        [DataMember] private Player _currentServer;
        public Player CurrentServer
        {
            get { return _currentServer; }
            set { this.RaiseAndSetIfChanged(ref _currentServer, value); }
        }

        [DataMember]
        private object _selectedPlayerOneAction;
        public object SelectedPlayerOneAction
        {
            get { return _selectedPlayerOneAction; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlayerOneAction, value); }
        }
        
        [DataMember]
        private ObservableAsPropertyHelper<string> _playerOneCurrentGame;
        public string PlayerOneCurrentGame
        {
            get { return _playerOneCurrentGame.Value; }
        }

        [DataMember]
        private Match _currMatch;
        public Match CurrMatch
        {
            get { return _currMatch; }
            set { this.RaiseAndSetIfChanged(ref _currMatch, value); }
        }
            
        [DataMember]
        private ObservableAsPropertyHelper<string> _playerTwoCurrentGame;
        public string PlayerTwoCurrentGame
        {
            get { return _playerTwoCurrentGame.Value; }
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

        [DataMember] private bool _playerOneIsServing;
        public bool PlayerOneIsServing
        {
            get { return _playerOneIsServing; }
            set { this.RaiseAndSetIfChanged(ref _playerOneIsServing, value); }
        }

        [DataMember] private bool _playerTwoIsServing;
        public bool PlayerTwoIsServing
        {
            get { return _playerTwoIsServing; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoIsServing, value); }
        }

        [DataMember]
        private ObservableAsPropertyHelper<string> _playerTwoThirdSet;
        public string PlayerTwoThirdSet
        {
            get { return _playerTwoThirdSet.Value; }
        }

        [DataMember]
        private ObservableAsPropertyHelper<string> _playerOneThirdSet;
        public string PlayerOneThirdSet
        {
            get { return _playerOneThirdSet.Value; }
        }

        [DataMember]
        private ObservableAsPropertyHelper<string> _playerOneSecondSet;
        public string PlayerOneSecondSet
        {
            get { return _playerOneSecondSet.Value; }
        }

        [DataMember]
        private ObservableAsPropertyHelper<string> _playerTwoSecondSet;
        public string PlayerTwoSecondSet
        {
            get { return _playerTwoSecondSet.Value; }
        }

        [DataMember]
        private ObservableAsPropertyHelper<string> _playerOneFirstSet;
        public string PlayerOneFirstSet
        {
            get { return _playerOneFirstSet.Value; }
        }

        [DataMember]
        private ObservableAsPropertyHelper<string> _playerTwoFirstSet;
        public string PlayerTwoFirstSet
        {
           get { return _playerTwoFirstSet.Value; }
        }

        [DataMember] private ObservableAsPropertyHelper<string> _matchStatus;
        public string MatchStatus
        {
            get { return _matchStatus.Value; }
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
