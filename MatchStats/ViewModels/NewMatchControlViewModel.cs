using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        private static List<string> _finalSetStrings;
        private FinalSetFormats _finalSetFormats;
        public NewMatchControlViewModel()
        {

            _finalSetStrings = new List<string>();
            _finalSetStrings.Add("10 Points Champs Tie Break");
            _finalSetStrings.Add("Normal Set");

            SaveCommand = new ReactiveCommand(IsValidForSave());
            SaveCommand.Subscribe(_ => SaveCommandImplementation());
        }

        private void SaveCommandImplementation()
        {
            MessageBus.Current.SendMessage(new Match());
        }

        public IObservable<bool> IsValidForSave()
        {
            //Combine change notification for required fields and push to SaveCommand when valid.
            return this.WhenAny(
                x => x.UseDefaultPlayer,
                x => x.PlayerOneFirstName,
                x => x.PlayerTwoFirstName,
                (defaultplayer, playerOneFname, playertwoFname) =>
                (
                    (defaultplayer.Value == true || (! string.IsNullOrEmpty(playerOneFname.Value))) &&
                    ! string.IsNullOrEmpty(playertwoFname.Value)

                ));
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

        [DataMember] private FinalSetFormats _selectedFinalSetFormat;
        public FinalSetFormats SelectedFinalSetFormat
        {
            get { return _selectedFinalSetFormat; }
            set { this.RaiseAndSetIfChanged(ref _selectedFinalSetFormat, value); }
        }

        [DataMember]
        private FinalSetFormats _selectedFinalSet;
        public FinalSetFormats SelectedFinalSet
        {
            get { return _selectedFinalSet; }
            set { this.RaiseAndSetIfChanged(ref _selectedFinalSet, value); }
        }

        [DataMember] private SetsFormat _selectedSetsFormat;
        public SetsFormat SelectedSetsFormat
        {
            get { return _selectedSetsFormat; }
            set { this.RaiseAndSetIfChanged(ref _selectedSetsFormat, value); }
        }

        [DataMember] private DueceFormat _selectedDueceFormat;
        public DueceFormat SelectedDueceFormat
        {
            get { return _selectedDueceFormat; }
            set { this.RaiseAndSetIfChanged(ref _selectedDueceFormat, value); }
        }

        [DataMember] private Grade _selecedGrade;
        public Grade SelectedGrade
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