using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;

namespace MatchStats.ViewModels
{
    [DataContract]
    public class MatchStatsViewModel : ReactiveObject, IRoutableViewModel
    {
        public MatchStatsViewModel(IScreen screen = null)
        {
            RandomGuid = Guid.NewGuid();
            UrlPathSegment = "MatchScore";
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();

        }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid
        {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        public List<Stat> Stats { get; set; }
        public int IndexWithinParentCollection { get; set; }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }

        public string TotalPointsWonByPlayerOne { get; set; }
        public string TotalPointsWonByPlayerTwo { get; set; }
    }

    public class Stat
    {
        public string StatName { get; set; }
        public string ForMatchP1 { get; set; }
        public string ForFirstSetP1 { get; set; }
        public string ForSecondSetP1 { get; set; }
        public string ForThirdSetP1 { get; set; }
        public int IndexWithinParentCollection { get; set; }
        public string ForMatchP2 { get; set; }
        public string ForThirdSetP2 { get; set; }
        public string ForSecondSetP2 { get; set; }
        public string ForFirstSetP2 { get; set; }
    }
}