using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using MatchStats.Enums;
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
        private FinalSetFormats _finalSetFormats;

        public NewMatchControlViewModel()
        {
            SaveCommand = new ReactiveCommand();
            SaveCommand.Subscribe(_ => SaveCommandImplementation());
        }

        private void SaveCommandImplementation()
        {
            MessageBus.Current.SendMessage(this);
        }

        public IReactiveCommand SaveCommand { get; protected set; }

        public List<FinalSetFormats> FinalSet
        {
            get { return _finalSet ?? (_finalSet = GetEnumAsList<FinalSetFormats>()); }
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

        public FinalSetFormats FinalSetFormat
        {
            get { return _finalSetFormats; }
            set { this.RaiseAndSetIfChanged(ref _finalSetFormats, value); }
        }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }

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