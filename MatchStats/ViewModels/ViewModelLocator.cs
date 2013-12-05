using MatchStats.Controls;
using MatchStats.DesignTimeStuff;
using MatchStats.Model;
using ReactiveUI;

namespace MatchStats.ViewModels
{

    public class ViewModelLocator
    {
        private MyMatchStats _myMatchStats;
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
                pageVm.ShowHideMatchPopup = true;
                pageVm.PlayerOnesName = "Ademola";
                pageVm.PlayerTwosName = "Kieran";
                pageVm.PlayerOneCurrentGame = "0";
                pageVm.PlayerTwoCurrentGame = "15";
                pageVm.PlayerOneFirstSet = "6";
                pageVm.PlayerTwoFirstSet = "4";
                return pageVm;
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
                homepageVm.MyMatchStats = new ReactiveList<MyMatchStats>();
                homepageVm.MyMatchStats.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView()); 
                homepageVm.DefaultPlayer = defaultPlayer;
                return homepageVm;
            }
        }

        //public UpcomingMatchesControlViewModel UpcomingMatchesControlViewModel
        //{
        //    get
        //    {
        //        return new DummyDataBuilder().BuildUpcomingMatchesDataForDesignView();
        //    }
        //}
    }
}