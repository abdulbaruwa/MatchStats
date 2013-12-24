using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
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
        private readonly IReactiveCommand addItemsCommand;

        public MatchScoreViewModel(IScreen screen = null)
        {
            PlayerOneActions = new ReactiveList<IGameActionViewModel>();
            PlayerTwoActions = new ReactiveList<IGameActionViewModel>();
            ScorePoints = new ReactiveList<IGameActionViewModel>();
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            UrlPathSegment = "MatchScore";
            NavToHomePageCommand = new ReactiveCommand();
            NavToHomePageCommand.Subscribe(_ => NavigateBackToHomePage());
            StartMatchCommand = new ReactiveCommand();
            StartMatchCommand.Subscribe(StartMatch);
            NewMatchControlViewModel = RxApp.DependencyResolver.GetService<NewMatchControlViewModel>();
            SetPlayerOneAsCurrentServerCommand = new ReactiveCommand(CanSetCurrentServerToPlayerOne());
            SetPlayerOneAsCurrentServerCommand.Subscribe(_ =>
            {
                CurrentMatch.Score.CurrentServer = CurrentMatch.PlayerOne;
                SaveMatch(CurrentMatch);
            });

            SetPlayerTwoAsCurrentServerCommand = new ReactiveCommand(CanSetCurrentServerToPlayerTwo());
            SetPlayerTwoAsCurrentServerCommand.Subscribe(_ =>
            {
                CurrentMatch.Score.CurrentServer = CurrentMatch.PlayerTwo;
                SaveMatch(CurrentMatch);
            });

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

            this.WhenAny(x => x.CurrMatch, x => x.Value)
                .Select(x => x)
                .ToProperty(this, x => x.CurrentMatch, new Match());

            _playerOneFirstSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.FirstOrDefault() != null)
                .Select(x => x.Sets.First().Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerOne.FullName).ToString())
                .ToProperty(this, x => x.PlayerOneFirstSet, "");
                
            _playerTwoFirstSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.FirstOrDefault() != null)
                .Select(x => x.Sets.First().Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerTwo.FullName).ToString())
                .ToProperty(this, x => x.PlayerTwoFirstSet, "");
                
            _playerOneSecondSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.SecondOrDefault() != null)
                .Select(x => x.Sets.SecondOrDefault().Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerOne.FullName).ToString())
                .ToProperty(this, x => x.PlayerOneFirstSet, "");
                
            _playerTwoSecondSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.SecondOrDefault() != null)
                .Select(x => x.Sets.First().Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerTwo.FullName).ToString())
                .ToProperty(this, x => x.PlayerTwoSecondSet, "");

            _playerOneThirdSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.ThirdOrDefault() != null)
                .Select(x => x.Sets.First().Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerOne.FullName).ToString())
                .ToProperty(this, x => x.PlayerOneThirdSet, "");

            _playerTwoThirdSet = this.WhenAny(x => x.CurrMatch.Score, x => x.Value)
                .Where(x => x.Sets.ThirdOrDefault() != null) //TODO: Need to 
                .Select(x => x.Sets.First().Games.Count(y => y.Winner != null && y.Winner.FullName == CurrMatch.PlayerTwo.FullName).ToString())
                .ToProperty(this, x => x.PlayerTwoThirdSet, "");
                
            MessageBus.Current.Listen<Match>("PointUpdateForCurrentMatch").Subscribe(x => CurrMatch = x);
        }

        private IObservable<bool> CanSetCurrentServerToPlayerOne()
        {
            return this.WhenAny(vm => vm.CurrMatch,
                (match) =>
                    (match.Value != null && match.Value.Score != null 
                                         && (match.Value.Score.CurrentServer == null || match.Value.Score.CurrentServer.FullName != match.Value.PlayerOne.FullName)));
        }

        private IObservable<bool> CanSetCurrentServerToPlayerTwo()
        {
            return this.WhenAny(vm => vm.CurrMatch,
                (match) =>
                    (match.Value != null && match.Value.Score != null 
                                         &&(match.Value.Score.CurrentServer == null || match.Value.Score.CurrentServer.FullName != match.Value.PlayerTwo.FullName)));
        }

        private  IObservable<IGameActionViewModel> GetGameCommandsForPlayer(Player player)
        {
            var listOfActions = new List<IGameActionViewModel>
            {
                new DoubleFaultCommandViewModel(player),
                new ForeHandWinnerCommandViewModel(player),
            };

            return listOfActions.ToObservable();
        }

        private void StartMatch(object param)
        {
            var match = param as Match;
            if (match != null)
            {
                SaveMatch(match);
                PlayerOneActions.Clear();

                GetGameCommandsForPlayer(match.PlayerOne).Subscribe(x => PlayerOneActions.Add(x));
                GetGameCommandsForPlayer(match.PlayerTwo).Subscribe(x => PlayerTwoActions.Add(x));
            }
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

        private ObservableAsPropertyHelper<List<IGameActionViewModel>> _playerOneActions;
        public List<IGameActionViewModel> PlayerOneCommands
        {
            get { return _playerOneActions.Value; }
        }

        [DataMember]
        private object _selectedPlayerOneAction;
        public object SelectedPlayerOneAction
        {
            get { return _selectedPlayerOneAction; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlayerOneAction, value); }
        }
        
        [DataMember]
        private string _playerOneCurrentGame = "";
        public string PlayerOneCurrentGame
        {
            get { return _playerOneCurrentGame; }
            set { this.RaiseAndSetIfChanged(ref _playerOneCurrentGame, value); }
        }


        [DataMember]
        private Match _currMatch;
        public Match CurrMatch
        {
            get { return _currMatch; }
            set { this.RaiseAndSetIfChanged(ref _currMatch, value); }
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