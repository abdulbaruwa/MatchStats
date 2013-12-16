using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.Media.PlayTo;
using MatchStats.Enums;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class NewMatchControlViewModel : ReactiveObject
    {
        private static List<FinalSetFormats> _finalSet;
        private static List<DueceFormat> _dueceFormat;
        private static List<SetsFormat> _setsFormat;
        private static List<Grade> _matchGrades;
        private static List<AgeGroup> _matchAgeGroups;
        private static List<Rating> _ratings;
        private static List<string> _finalSetStrings;
        private FinalSetFormats _finalSetFormats;
        public NewMatchControlViewModel()
        {
            SaveCommand = new ReactiveCommand(IsValidForSave());
            SaveCommand.Subscribe(_ => SaveCommandImplementation());
            _showMe = true;
        }

        private void SaveCommandImplementation()
        {
            var match = new Match(){MatchGuid = Guid.NewGuid(), MatchTime = DateTime.Now};
            match.Score = new Score()
            {
                GameOne = new Game()
            };
            match.MatchFormat = new MatchFormat()
            {
                DueceFormat = (DueceFormat) this.SelectedDueceFormat,
                FinalSetType = (FinalSetFormats) this.SelectedFinalSet,
                SetsFormat = (SetsFormat) this.SelectedSetsFormat
            };

            if (! UseDefaultPlayer)
            {
                match.PlayerOne = BuildPlayerOne();
            }

            var tournament = new Tournament();
            if (! string.IsNullOrEmpty(TournamentName))
            {
                tournament.TournamentName = TournamentName;
                if (SelectedGrade != null)
                {
                    tournament.TournamentGrade = SelectedGrade.ToString();
                }
            }

            match.Tournament = tournament;
            match.PlayerTwo = BuildPlayerTwo();
            SavedMatch = match;
            ShowMe = false;
            //MessageBus.Current.SendMessage(match);
        }

        private Player BuildPlayerOne()
        {
            var player = new Player();
            player.FirstName = PlayerOneFirstName;
            player.SurName = PlayerOneLastName;
            if (SelectedPlayerOneRating != null)
            {
                player.Rating = SelectedPlayerOneRating.ToString();
            }
            return player;
        }

        private Player BuildPlayerTwo()
        {
            var player = new Player();
            player.FirstName = PlayerTwoFirstName;
            player.SurName = PlayerTwoLastName;
            if (SelectedPlayerTwoRating != null)
            {
                player.Rating = SelectedPlayerTwoRating.ToString();
            }
            return player;
        }


        public IObservable<bool> IsValidForSave()
        {
            //Combine change notification for required fields and push to SaveCommand when valid.
            return this.WhenAny(
                x => x.UseDefaultPlayer,
                x => x.PlayerOneFirstName,
                x => x.PlayerTwoFirstName,
                x => x.SelectedDueceFormat,
                x => x.SelectedFinalSet,
                x => x.SelectedSetsFormat,
                x => x.TournamentName,
                (defaultplayer, playerOneFname, playertwoFname, duece, finalset, sets, tournament) =>
                (
                    duece.Value != null && finalset.Value != null && sets.Value != null && ! string.IsNullOrWhiteSpace(tournament.Value) &&
                    (defaultplayer.Value == true || (! string.IsNullOrEmpty(playerOneFname.Value))) &&
                    ! string.IsNullOrEmpty(playertwoFname.Value)

                ));
        }
        
        [DataMember]
        private bool _showMe;
        public bool ShowMe
        {
            get { return _showMe; }
            set { this.RaiseAndSetIfChanged(ref _showMe, value); }
        }

        [DataMember]
        private Match _savedMatch;
        public Match SavedMatch
        {
            get { return _savedMatch; }
            set { this.RaiseAndSetIfChanged(ref _savedMatch, value); }
        }
        public IReactiveCommand SaveCommand { get; protected set; }

        public IEnumerable<string> FinalSetStrings
        {
            get
            {
                return _finalSetStrings;
            }
        }
        public List<FinalSetFormats> FinalSet
        {
            get
            {
               var result = _finalSet ?? (_finalSet = GetEnumAsList<FinalSetFormats>());
                return result;
            }
        }

        public List<DueceFormat> DueceFormats
        {
            get { return _dueceFormat ?? (_dueceFormat = GetEnumAsList<DueceFormat>()); }
        }

        public List<SetsFormat> SetsFormats
        {
            get { return _setsFormat ?? (_setsFormat = GetEnumAsList<SetsFormat>()); }
        }

        public List<Grade> Grades
        {
            get { return _matchGrades ?? (_matchGrades = GetEnumAsList<Grade>()); }
        }

        public List<AgeGroup> AgeGroups
        {
            get { return _matchAgeGroups ?? (_matchAgeGroups = GetEnumAsList<AgeGroup>()); }
        }

        public List<Rating> Ratings
        {
            get { return _ratings ?? (_ratings = GetEnumAsList<Rating>()); }
        }

        //ObservableAsPropertyHelper<Song> _CurrentSong;
        //public Song CurrentSong
        //{
        //    get { return _CurrentSong.Value; }
        //}

        public FinalSetFormats FinalSetFormat
        {
            get { return _finalSetFormats; }
            set { this.RaiseAndSetIfChanged(ref _finalSetFormats, value); }
        }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }

        private bool _useDefaultPlayer;
        public bool UseDefaultPlayer
        {
            get { return _useDefaultPlayer; }
            set { this.RaiseAndSetIfChanged(ref _useDefaultPlayer, value);}
        }

        [DataMember]
        private string _playerOneFirstName = ""; 
        public string PlayerOneFirstName
        {
            get { return _playerOneFirstName; }
            set { this.RaiseAndSetIfChanged(ref _playerOneFirstName, value); }
        }

        [DataMember] private string _playerOneLastName = "";
        public string PlayerOneLastName
        {
            get { return _playerOneLastName; }
            set { this.RaiseAndSetIfChanged(ref _playerOneLastName, value); }
        }

        [DataMember] private string _playerTwoFirstName ="";
        public string PlayerTwoFirstName
        {
            get { return _playerTwoFirstName; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoFirstName, value); }
        }

        [DataMember] private string _playerTwoLastName = "";
        public string PlayerTwoLastName
        {
            get { return _playerTwoLastName; }
            set { this.RaiseAndSetIfChanged(ref _playerTwoLastName, value); }
        }

        [DataMember]
        private object _selectedPlayerOneRating;
        public object SelectedPlayerOneRating
        {
            get { return _selectedPlayerOneRating; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlayerOneRating, value); }
        }

        [DataMember]
        private object _selectedPlayerTwoRating;
        public object SelectedPlayerTwoRating
        {
            get { return _selectedPlayerTwoRating; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlayerTwoRating, value); }
        }

        [DataMember] private string _tounamentName = string.Empty;
        public string TournamentName
        {
            get { return _tounamentName; }
            set { this.RaiseAndSetIfChanged(ref _tounamentName, value); }
        }

        [DataMember] private AgeGroup _selectedAgeGroup;
        public AgeGroup SelectedAgeGroup
        {
            get { return _selectedAgeGroup; }
            set { this.RaiseAndSetIfChanged(ref _selectedAgeGroup, value); }
        }

        [DataMember]
        private object _selectedFinalSet;
        public object SelectedFinalSet
        {
            get { return _selectedFinalSet; }
            set { this.RaiseAndSetIfChanged(ref _selectedFinalSet, value); }
        }

        [DataMember] private object _selectedSetsFormat;
        public object SelectedSetsFormat
        {
            get { return _selectedSetsFormat; }
            set { this.RaiseAndSetIfChanged(ref _selectedSetsFormat, value); }
        }

        [DataMember] private object _selectedDueceFormat;
        public object SelectedDueceFormat
        {
            get { return _selectedDueceFormat; }
            set { this.RaiseAndSetIfChanged(ref _selectedDueceFormat, value); }
        }

        [DataMember] private object _selecedGrade;
        public object SelectedGrade
        {
            get { return _selecedGrade; }
            set { this.RaiseAndSetIfChanged(ref _selecedGrade, value); }
        }

        private List<T> GetEnumAsList<T>()
        {
            Array enumArray = Enum.GetValues(typeof (T));
            var result = new List<T>();
            foreach (object item in enumArray)
            {
                result.Add((T) item);
            }
            return result;
        }
    }
}