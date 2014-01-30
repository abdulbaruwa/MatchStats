using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
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
                   // AddFirstServePercentageStats();
                });
        }

        private void UpdateFields()

        {
            PlayerOneFullName = CurrentMatch.PlayerOne.FullName;
            PlayerTwoFullName = CurrentMatch.PlayerTwo.FullName;
            if (CurrentMatch.Score.Sets.FirstOrDefault() != null)
                FirstSetDuration = CurrentMatch.Score.Sets.FirstOrDefault().DurationInMinutes + "mins";
            if (CurrentMatch.Score.Sets.SecondOrDefault() != null)
                SecondSetDuration = CurrentMatch.Score.Sets.SecondOrDefault().DurationInMinutes + "mins";
            if (CurrentMatch.Score.Sets.ThirdOrDefault() != null)
                ThirdSetDuration = CurrentMatch.Score.Sets.ThirdOrDefault().DurationInMinutes + "mins";

            if (CurrentMatch.Score.Sets.FirstOrDefault() != null)
            {
                var playerOneScore = CurrentMatch.Score.Sets.First().Games.Count(x => x.Winner.FullName == PlayerOneFullName);
                var playerTwoScore = CurrentMatch.Score.Sets.First().Games.Count(x => x.Winner.FullName == PlayerTwoFullName);
                PlayerOneSetOneScore = playerOneScore.ToString();
                PlayerTwoSetOneScore = playerTwoScore.ToString();
                if (playerOneScore.DiffValueWith(playerTwoScore) == 1)
                {
                    //A tiebreaker was played
                    PlayerOneSetOneTiebreakScore = CurrentMatch.Score.Sets.First().Games.Last().PlayerOneScore.ToString();
                    PlayerTwoSetOneTiebreakScore = CurrentMatch.Score.Sets.First().Games.Last().PlayerTwoScore.ToString();
                }
            }

            if (CurrentMatch.Score.Sets.SecondOrDefault() != null)
            {
                var playerOneScore = CurrentMatch.Score.Sets.SecondOrDefault().Games.Count(x => x.Winner.FullName == PlayerOneFullName);
                var playerTwoScore = CurrentMatch.Score.Sets.SecondOrDefault().Games.Count(x => x.Winner.FullName == PlayerTwoFullName);
                PlayerOneSetTwoScore = playerOneScore.ToString();
                PlayerTwoSetTwoScore = playerTwoScore.ToString();
                if (playerOneScore.DiffValueWith(playerTwoScore) == 1)
                {
                    //A tiebreaker was played
                    PlayerOneSetTwoTiebreakScore = CurrentMatch.Score.Sets.SecondOrDefault().Games.Last().PlayerOneScore.ToString();
                    PlayerTwoSetTwoTiebreakScore = CurrentMatch.Score.Sets.SecondOrDefault().Games.Last().PlayerTwoScore.ToString();
                }
            }

            if (CurrentMatch.Score.Sets.ThirdOrDefault() != null)
            {
                var playerOneScore = CurrentMatch.Score.Sets.ThirdOrDefault().Games.Count(x => x.Winner.FullName == PlayerOneFullName);
                var playerTwoScore = CurrentMatch.Score.Sets.ThirdOrDefault().Games.Count(x => x.Winner.FullName == PlayerTwoFullName);
                PlayerOneSetThreeScore = playerOneScore.ToString();
                PlayerTwoSetThreeScore = playerTwoScore.ToString();

                if (CurrentMatch.MatchFormat.FinalSetType == FinalSetFormats.Normal && (playerOneScore.DiffValueWith(playerTwoScore) == 1))
                {
                    //Finanal set was a normal set and a normal tiebreaker was played
                    PlayerOneSetTwoTiebreakScore = CurrentMatch.Score.Sets.ThirdOrDefault().Games.Last().PlayerOneScore.ToString();
                    PlayerTwoSetTwoTiebreakScore = CurrentMatch.Score.Sets.ThirdOrDefault().Games.Last().PlayerTwoScore.ToString();
                }
            }

            TotalPointsWonByPlayerOne = CurrentMatch.Score.Sets.Sum(x => x.Games.Sum(y => y.PlayerOneScore)).ToString();
            TotalPointsWonByPlayerTwo = CurrentMatch.Score.Sets.Sum(x => x.Games.Sum(y => y.PlayerTwoScore)).ToString();


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

        private void AddFirstServePercentageStats()
        {
            var firstServes = this.CurrentMatch.MatchStats.Where(x => x.Reason == StatDescription.FirstServeIn
                || x.Reason == StatDescription.FirstServeOut || x.Reason == StatDescription.FirstServeAce && x.Player.IsPlayerOne).ToList();
            var firstServesIn = firstServes.Where(x => x.Reason == StatDescription.FirstServeAce || x.Reason == StatDescription.FirstServeIn).ToList();
            var percentageFirstServer =
                (int) Math.Round(((double) firstServesIn.Count())/((double) firstServes.Count())*100);

            var firstServesP2 = this.CurrentMatch.MatchStats.Where(x => x.Reason == StatDescription.FirstServeIn
                || x.Reason == StatDescription.FirstServeOut || x.Reason == StatDescription.FirstServeAce && (x.Player.IsPlayerOne == false)).ToList();
            var firstServesInP2 = firstServes.Where(x => x.Reason == StatDescription.FirstServeAce || x.Reason == StatDescription.FirstServeIn).ToList();
            var  percentageFirstServerP2 = (int) Math.Round(((double)firstServesInP2.Count()) / ((double)firstServesP2.Count()) * 100);

            var firstServe = Stats.FirstOrDefault(x => x.StatName == "First Serve %");

            if (firstServe == null)
            {
                Stats.Add(new Stat(){StatName = "First Serve %", ForMatchP1 = percentageFirstServer + "%", ForMatchP2 = percentageFirstServerP2 + "%"});
            }
            else
            {
                firstServe.StatName = "First Serve %";
                firstServe.ForMatchP1 = percentageFirstServer + "%";
                firstServe.ForMatchP2 = percentageFirstServerP2 + "%";
            }
        }
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
