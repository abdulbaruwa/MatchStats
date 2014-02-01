using System.Collections.Generic;
using MatchStats.DesignTimeStuff;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{

    public class ViewModelLocator
    {
        private Player _defaultPlayer;
        public ViewModelLocator()
        {

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;
        }

        public MatchScoreViewModel MatchScoreViewModel
        {
            get
            {
                var pageVm = new MatchScoreViewModel();
                for (int i = 0; i < 10; i++)
                {
                    pageVm.ScorePoints.Add(new ScorePoint() { Name = "Action " + i, Player = new Player(){FirstName =  "Ademola" }});
                }

                return pageVm;
            }
        }

        public MatchStatsViewModel MatchStatsViewModel
        {
            get
            {
                var matchStatsViewModel = new MatchStatsViewModel();
                matchStatsViewModel.TotalPointsWonByPlayerOne = "112";
                matchStatsViewModel.TotalPointsWonByPlayerTwo = "98";

                matchStatsViewModel.PlayerOneFullName = "Rafael Nadal";
                matchStatsViewModel.PlayerTwoFullName = "Kiran Nishikori";
                matchStatsViewModel.PlayerOneSetOneScore = "7";
                matchStatsViewModel.PlayerOneSetOneTiebreakScore = "7";
                matchStatsViewModel.PlayerOneSetTwoScore = "7";
                matchStatsViewModel.PlayerOneSetThreeScore = "6";

                matchStatsViewModel.PlayerTwoSetOneScore = "6";
                matchStatsViewModel.PlayerTwoSetOneTiebreakScore = "3";

                matchStatsViewModel.PlayerTwoSetTwoScore = "5";
                matchStatsViewModel.PlayerTwoSetThreeScore = "4";

                matchStatsViewModel.FirstSetDuration = "47mins";
                matchStatsViewModel.SecondSetDuration = "53mins";
                matchStatsViewModel.ThirdSetDuration = "39mins";

                matchStatsViewModel.Stats = new List<Stat>();
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.Aces, ForMatchP1 = "18", ForMatchP2 = "1", ForFirstSetP1 = "9", ForFirstSetP2 = "2", ForSecondSetP1 = "6", ForSecondSetP2 = "4", ForThirdSetP1 = "2", ForThirdSetP2 = "0", IndexWithinParentCollection = 0});
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.FirstServePercentage,  ForMatchP1 = "70%", ForMatchP2 = "65%", ForFirstSetP1 = "77%", ForFirstSetP2 = "62%", ForSecondSetP1 = "66%", ForSecondSetP2 = "60%", ForThirdSetP1 = "64%", ForThirdSetP2 = "81%", IndexWithinParentCollection = 1});
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.DoubleFaults, ForMatchP1 = "1", ForMatchP2 = "1", ForFirstSetP1 = "0", ForFirstSetP2 = "0", ForSecondSetP1 = "0", ForSecondSetP2 = "1", ForThirdSetP1 = "1", ForThirdSetP2 = "0",IndexWithinParentCollection = 2});
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.WinPercentForFirstServe, ForMatchP1 = "76%", ForMatchP2 = "69%", ForFirstSetP1 = "78%", ForFirstSetP2 = "65%", ForSecondSetP1 = "81%", ForSecondSetP2 = "71%", ForThirdSetP1 = "67%", ForThirdSetP2 = "71%",IndexWithinParentCollection = 3});
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.WinPercentForSecondServe, ForMatchP1 = "38%", ForMatchP2 = "41%", ForFirstSetP1 = "27%", ForFirstSetP2 = "50%", ForSecondSetP1 = "55%", ForSecondSetP2 = "36%", ForThirdSetP1 = "33%", ForThirdSetP2 = "25%", IndexWithinParentCollection = 4});
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.Winners, ForMatchP1 = "31", ForMatchP2 = "32", ForFirstSetP1 = "13", ForFirstSetP2 = "10", ForSecondSetP1 = "11", ForSecondSetP2 = "12", ForThirdSetP1 = "7", ForThirdSetP2 = "10", IndexWithinParentCollection = 5 });
                matchStatsViewModel.Stats.Add(new Stat() {StatNameType = StatName.UnforcedErrors, ForMatchP1 = "26", ForMatchP2 = "43", ForFirstSetP1 = "13", ForFirstSetP2 = "17", ForSecondSetP1 = "9", ForSecondSetP2 = "16", ForThirdSetP1 = "4", ForThirdSetP2 = "10",IndexWithinParentCollection = 6});
                return matchStatsViewModel;
            }
        }


        public NewMatchControlViewModel NewMatchControlViewModel
        {
            get
            {
                var newMatchControlVM = new NewMatchControlViewModel();
                newMatchControlVM.UseDefaultPlayer = true;
                return newMatchControlVM;
            }
        }

        public MatchesPlayedViewModel MatchesPlayedViewModel
        {
            get
            {
                var defaultPlayer = new Player() { FirstName = "Ademola", Rating = "7.2", SurName = "Baruwa" };
                var homepageVm = new MatchesPlayedViewModel(new AppBootstrapper());
                homepageVm.MyMatchStats = new ReactiveList<Match>();
                homepageVm.MyMatchStats.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView()); 
                homepageVm.DefaultPlayer = defaultPlayer;
                return homepageVm;
            }
        }

    }
}
