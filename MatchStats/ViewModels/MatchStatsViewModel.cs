using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Windows.Devices.Sensors;
using MatchStats.Common;
using MatchStats.Enums;
using MatchStats.Model;
using ReactiveUI;
using WinRTXamlToolkit.Tools;

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
            Stats = new List<Stat>();
            InitializeFields();
        }

        private void InitializeFields()
        {
            this.WhenAny(x => x.CurrentMatch, x => x.Value)
                .Where(x => x != null)
                .Subscribe(_ =>
                {
                    UpdateFields();
                    AddFirstServePercentageStats();
                    AddDoubleFaultStats();
                    AddWinPercentateOnFirstServe();
                });
        }

        private void UpdateFields()

        {
            var set1 = this.CurrentMatch.Sets.FirstOrDefault();
            if (set1 != null) Set1 = set1.SetId;
            var set2 = this.CurrentMatch.Sets.FirstOrDefault();
            if (set2 != null) Set2 = set2.SetId;
            var set3 = this.CurrentMatch.Sets.FirstOrDefault();
            if (set3 != null) Set3 = set3.SetId;

            PlayerOneFullName = CurrentMatch.PlayerOne.FullName;
            PlayerTwoFullName = CurrentMatch.PlayerTwo.FullName;
            if (CurrentMatch.Sets.FirstOrDefault() != null)
                FirstSetDuration = CurrentMatch.Sets.FirstOrDefault().DurationInMinutes + "mins";
            if (CurrentMatch.Sets.SecondOrDefault() != null)
                SecondSetDuration = CurrentMatch.Sets.SecondOrDefault().DurationInMinutes + "mins";
            if (CurrentMatch.Sets.ThirdOrDefault() != null)
                ThirdSetDuration = CurrentMatch.Sets.ThirdOrDefault().DurationInMinutes + "mins";

            if (CurrentMatch.Sets.FirstOrDefault() != null)
            {
                var playerOneScore = CurrentMatch.Sets.First().Games.Count(x => x.Winner != null && x.Winner.FullName == PlayerOneFullName);
                var playerTwoScore = CurrentMatch.Sets.First().Games.Count(x => x.Winner != null && x.Winner.FullName == PlayerTwoFullName);
                PlayerOneSetOneScore = playerOneScore.ToString();
                PlayerTwoSetOneScore = playerTwoScore.ToString();
                if (playerOneScore.DiffValueWith(playerTwoScore) == 1)
                {
                    //A tiebreaker was played
                    PlayerOneSetOneTiebreakScore = CurrentMatch.Sets.First().Games.Last().PlayerOneScore.ToString();
                    PlayerTwoSetOneTiebreakScore = CurrentMatch.Sets.First().Games.Last().PlayerTwoScore.ToString();
                }
            }

            if (CurrentMatch.Sets.SecondOrDefault() != null)
            {
                var playerOneScore = CurrentMatch.Sets.SecondOrDefault().Games.Count(x => x.Winner != null && x.Winner.FullName == PlayerOneFullName);
                var playerTwoScore = CurrentMatch.Sets.SecondOrDefault().Games.Count(x => x.Winner != null && x.Winner.FullName == PlayerTwoFullName);
                PlayerOneSetTwoScore = playerOneScore.ToString();
                PlayerTwoSetTwoScore = playerTwoScore.ToString();
                if (playerOneScore.DiffValueWith(playerTwoScore) == 1)
                {
                    //A tiebreaker was played
                    PlayerOneSetTwoTiebreakScore = CurrentMatch.Sets.SecondOrDefault().Games.Last().PlayerOneScore.ToString();
                    PlayerTwoSetTwoTiebreakScore = CurrentMatch.Sets.SecondOrDefault().Games.Last().PlayerTwoScore.ToString();
                }
            }

            if (CurrentMatch.Sets.ThirdOrDefault() != null)
            {
                var playerOneScore = CurrentMatch.Sets.ThirdOrDefault().Games.Count(x => x.Winner != null && x.Winner.FullName == PlayerOneFullName);
                var playerTwoScore = CurrentMatch.Sets.ThirdOrDefault().Games.Count(x => x.Winner != null && x.Winner.FullName == PlayerTwoFullName);
                PlayerOneSetThreeScore = playerOneScore.ToString();
                PlayerTwoSetThreeScore = playerTwoScore.ToString();

                if (CurrentMatch.MatchFormat.FinalSetType == FinalSetFormats.Normal && (playerOneScore.DiffValueWith(playerTwoScore) == 1))
                {
                    //Finanal set was a normal set and a normal tiebreaker was played
                    PlayerOneSetTwoTiebreakScore = CurrentMatch.Sets.ThirdOrDefault().Games.Last().PlayerOneScore.ToString();
                    PlayerTwoSetTwoTiebreakScore = CurrentMatch.Sets.ThirdOrDefault().Games.Last().PlayerTwoScore.ToString();
                }
            }

            TotalPointsWonByPlayerOne = CurrentMatch.Sets.Sum(x => x.Games.Sum(y => y.PlayerOneScore)).ToString();
            TotalPointsWonByPlayerTwo = CurrentMatch.Sets.Sum(x => x.Games.Sum(y => y.PlayerTwoScore)).ToString();


        }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid
        {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        [DataMember]
        private Match _currentMatch;
        public Match CurrentMatch
        {
            get { return _currentMatch; }
            set { this.RaiseAndSetIfChanged(ref _currentMatch, value); }
        }

        private string Set1 { get; set; }
        private string Set2 { get; set; }
        private string Set3 { get; set; }

        public List<Stat> Stats { get; set; }
        public int IndexWithinParentCollection { get; set; }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }

        public string TotalPointsWonByPlayerOne { get; set; }
        public string TotalPointsWonByPlayerTwo { get; set; }

        public string PlayerOneFullName { get; set; }
        public string PlayerTwoFullName { get; set; }

        public string PlayerOneSetOneScore { get; set; }
        public string PlayerOneSetTwoScore { get; set; }
        public string PlayerOneSetThreeScore { get; set; }

        public string PlayerTwoSetOneScore { get; set; }
        public string PlayerTwoSetTwoScore { get; set; }
        public string PlayerTwoSetThreeScore { get; set; }

        public string PlayerOneSetOneTiebreakScore { get; set; }
        public string PlayerOneSetTwoTiebreakScore { get; set; }
        public string PlayerOneSetThreeTiebreakScore { get; set; }
        public string PlayerTwoSetOneTiebreakScore { get; set; }
        public string PlayerTwoSetTwoTiebreakScore { get; set; }
        public string PlayerTwoSetThreeTiebreakScore { get; set; }

        public string FirstSetDuration { get; set; }
        public string SecondSetDuration { get; set; }
        public string ThirdSetDuration { get; set; }


        private void AddWinPercentateOnFirstServe()
        {
            var pointsPercWonOnFirstServePlayerOne = GetFirstServeWinPercentageFor(true);
            var pointsPercWonOnFirstServePlayerTwo = GetFirstServeWinPercentageFor(false);
            Stats.Add(new Stat()
            {
                ForMatchP1 = pointsPercWonOnFirstServePlayerOne,
                ForMatchP2 = pointsPercWonOnFirstServePlayerTwo,
                StatNameType = StatName.WinPercentForFirstServe,
                ForFirstSetP1 = string.IsNullOrEmpty(Set1) ? "" : GetFirstServeWinPercentageFor(true, Set1),
                ForFirstSetP2 = string.IsNullOrEmpty(Set1) ? "" : GetFirstServeWinPercentageFor(false, Set1),
                ForSecondSetP1 = string.IsNullOrEmpty(Set2) ? "" : GetFirstServeWinPercentageFor(true, Set2),
                ForSecondSetP2 = string.IsNullOrEmpty(Set2) ? "" : GetFirstServeWinPercentageFor(false, Set2),
                ForThirdSetP1 = string.IsNullOrEmpty(Set3) ? "" : GetFirstServeWinPercentageFor(true, Set3),
                ForThirdSetP2 = string.IsNullOrEmpty(Set3) ? "" : GetFirstServeWinPercentageFor(false, Set3),
            });
        }

        private string GetFirstServeWinPercentageFor(bool isPlayerOne, string set = null)
        {
            List<Point> firstServes;
            if (string.IsNullOrEmpty(set))
            {
                firstServes = (from s in CurrentMatch.Sets
                               from g in s.Games
                               from p in g.Points
                               where p.Serves.Any(x => x.ServeIsIn)
                               select p).ToList();

            }
            else
            {
                firstServes = (from s in CurrentMatch.Sets
                               from g in s.Games
                               from p in g.Points
                               where p.Serves.Any(x => x.ServeIsIn && s.SetId == set)
                               select p).ToList();
            }

            var pointsWonOnfirstServe =
                firstServes.Count(x => x.Server.IsPlayerOne == isPlayerOne && x.Player.IsPlayerOne == isPlayerOne);

            var winPercentageOnFirstServe = (int)Math.Round(((double)pointsWonOnfirstServe / (double)firstServes.Count) * 100);

            return winPercentageOnFirstServe + "%";
        }

        private void AddDoubleFaultStats()
        {

            var secondServes = (from s in CurrentMatch.Sets
                               from g in s.Games
                               from p in g.Points
                               from sr in p.Serves
                               where sr.IsFirstServe == false && sr.ServeIsIn == false
                               select sr).ToList();

            var doubleFaultsP1 = secondServes.Count(x => x.Server.IsPlayerOne);
            var doubleFaultsP2 = secondServes.Count(x => x.Server.IsPlayerOne == false);

            var set1 = this.CurrentMatch.Sets.FirstOrDefault();
            var playerOneSet1DoubleFaults = GetDoubleFaults(true, set1);
            var playerTwoSet1DoubleFaults = GetDoubleFaults(false, set1);

            var set2 = this.CurrentMatch.Sets.SecondOrDefault();
            var playerOneSet2DoubleFaults = GetDoubleFaults(true, set2);
            var playerTwoSet2DoubleFaults = GetDoubleFaults(false, set2); 

            var set3 = this.CurrentMatch.Sets.ThirdOrDefault();
            var playerOneSet3DoubleFaults = GetDoubleFaults(true, set3);
            var playerTwoSet3DoubleFaults = GetDoubleFaults(false, set3);

            Stats.Add(new Stat(){ForMatchP1 = doubleFaultsP1.ToString(), 
                ForMatchP2 = doubleFaultsP2.ToString(), 
                StatNameType = StatName.DoubleFaults,
                ForFirstSetP1 = playerOneSet1DoubleFaults,
                ForFirstSetP2 = playerTwoSet1DoubleFaults,
                ForSecondSetP1 = playerOneSet2DoubleFaults,
                ForSecondSetP2 = playerTwoSet2DoubleFaults,
                ForThirdSetP1 = playerOneSet3DoubleFaults,
                ForThirdSetP2 = playerTwoSet3DoubleFaults,
            });
        }

        private string GetDoubleFaults(bool isPlayerOne, Set set)
        {
            if (set == null) return string.Empty;
            return (from s in CurrentMatch.Sets
                                from g in s.Games
                                from p in g.Points
                                from sr in p.Serves
                                where sr.IsFirstServe == false && sr.ServeIsIn == false && s.SetId == set.SetId && sr.Server.IsPlayerOne == isPlayerOne
                                select sr).Count().ToString();
        }

        private void AddFirstServePercentageStats()
        {
            var firstServes = (from s in CurrentMatch.Sets
                              from g in s.Games
                              from p in g.Points
                              from sr in p.Serves
                              where sr.IsFirstServe
                              select sr).ToList();

            var playerOneFirstServesIn = firstServes.Where(x => x.ServeIsIn && x.Server.IsPlayerOne);
            var percentageFirstServer = (int)Math.Round(((double)playerOneFirstServesIn.Count()) / ((double)firstServes.Count(x => x.Server.IsPlayerOne)) * 100);

            var firstServesP2 = firstServes.Where(x => x.ServeIsIn && x.Server.IsPlayerOne == false);
            var percentageFirstServerP2 = (int)Math.Round(firstServesP2.Count() / ((double)firstServes.Count(x => x.Server.IsPlayerOne == false)) * 100);

            var firstServe = Stats.FirstOrDefault(x => x.StatNameType == StatName.FirstServePercentage);
            
            var playerOneFirstServeSet1Percentage = GetPlayerOneFirstServePercentate(true);
            var playerTwoFirstServeSet1Percentage = GetPlayerOneFirstServePercentate(false);            
            
            var playerOneServeSet2Percentage =  GetPlayerOneSecondServePercentate(true);
            var playerTwoServeSet2Percentage =  GetPlayerOneSecondServePercentate(false);

            var playerOneServeSet3Percentage =  GetPlayerOneThirdServePercentate(true);
            var playerTwoServeSet3Percentage =  GetPlayerOneThirdServePercentate(false);

            if (firstServe == null)
            {
                Stats.Add(new Stat()
                {
                    StatNameType = StatName.FirstServePercentage,
                    ForMatchP1 = percentageFirstServer + "%",
                    ForMatchP2 = percentageFirstServerP2 + "%",
                    ForFirstSetP1 = playerOneFirstServeSet1Percentage,
                    ForFirstSetP2 = playerTwoFirstServeSet1Percentage,
                    ForSecondSetP1 = playerOneServeSet2Percentage,
                    ForSecondSetP2 = playerTwoServeSet2Percentage,
                    ForThirdSetP1 =  playerOneServeSet3Percentage,
                    ForThirdSetP2 = playerTwoServeSet3Percentage
                });
            }
            else
            {
                firstServe.StatNameType = StatName.FirstServePercentage;
                firstServe.ForMatchP1 = percentageFirstServer + "%";
                firstServe.ForMatchP2 = percentageFirstServerP2 + "%";
                firstServe.ForFirstSetP1 = playerOneFirstServeSet1Percentage;
                firstServe.ForFirstSetP2 = playerTwoFirstServeSet1Percentage;
                firstServe.ForSecondSetP1 = playerOneServeSet2Percentage;
                firstServe.ForSecondSetP2 = playerTwoServeSet2Percentage;
                firstServe.ForThirdSetP1 = playerOneServeSet3Percentage;
                firstServe.ForThirdSetP2 = playerTwoServeSet3Percentage;
            }
        }

        private string GetPlayerOneThirdServePercentate(bool isPlayerOne)
        {
            var firstSetId = CurrentMatch.Sets.ThirdOrDefault();
            return firstSetId == null ? string.Empty : GetPlayerFirstServePercentate(isPlayerOne, firstSetId.SetId);
        }

        private string GetPlayerOneSecondServePercentate(bool isPlayerOne)
        {
            var firstSetId = CurrentMatch.Sets.SecondOrDefault();
            return firstSetId == null ? string.Empty : GetPlayerFirstServePercentate(isPlayerOne, firstSetId.SetId);
        }

        private string GetPlayerOneFirstServePercentate(bool b)
        {
            var firstSetId = CurrentMatch.Sets.FirstOrDefault();
            return firstSetId == null ? string.Empty : GetPlayerFirstServePercentate(b, firstSetId.SetId);
        }

        private string GetPlayerFirstServePercentate(bool isPlayerOne, string firstSetId)
        {
            var firstServes = (from s in CurrentMatch.Sets
                               from g in s.Games
                               from p in g.Points
                               from sr in p.Serves
                               where sr.IsFirstServe && s.SetId == firstSetId && sr.Server.IsPlayerOne == isPlayerOne
                               select sr).ToList();

            var servesIn = firstServes.Where(x => x.ServeIsIn).ToList();
            
            var percentageFirstServer = (int)Math.Round(((double)servesIn.Count()) / ((double)firstServes.Count()) * 100);

            return percentageFirstServer + "%";
        }
    }
 
    public class Stat
    {
        public StatName StatNameType { get;set; }
        public string StatName {
            get { return StatNameType.GetAttribute<DisplayAttribute>().Name; }
        }
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
