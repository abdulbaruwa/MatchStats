using System.Diagnostics.Tracing;
using System.Linq;
using Akavache;
using MatchStats.Enums;
using MatchStats.Logging;
using MatchStats.Model;
using MatchStats.ViewModels;
using ReactiveUI;

namespace MatchStats.Test.ViewModels.MatchScoreViewModelTests
{
    public static class MatchScoreViewModelTestHelper
    {
        public static MatchScoreViewModel CreateUnSavedMatchScoreViewModelFixture()
        {
            var blobCache = RegisterComponents();
            var fixture = BuildAMatchToScore();

            return fixture;
        }
        public static MatchScoreViewModel CreateNewMatchFixture()
        {
            var fixture = CreateUnSavedMatchScoreViewModelFixture();
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            fixture.GameIsOnGoing = true;
            return fixture;
        }

        public static MatchScoreViewModel CreateNewMatchFixture(FinalSetFormats finalSetFormat)
        {
            var fixture = CreateUnSavedMatchScoreViewModelFixture();
            fixture.GameIsOnGoing = true;
            fixture.NewMatchControlViewModel.SelectedFinalSet = finalSetFormat;
            fixture.NewMatchControlViewModel.SaveCommand.Execute(null);
            return fixture;
        }

        public static void AddASetForPlayer(MatchScoreViewModel fixture, bool IsPlayerOne)
        {
            var sets = (int)fixture.CurrMatch.MatchFormat.SetsFormat;
            for (int i = 0; i < sets; i++)
            {
                if (IsPlayerOne)
                {
                    AddAGameForPlayerOne(fixture);
                }
                else
                {
                    AddAGameForPlayerTwo(fixture);
                }
            }
        }


        public static void AddAGameForPlayerOne(MatchScoreViewModel fixture)
        {
            PlayerFirstServedAndForehandWinner(fixture, true);
            PlayerFirstServedAndForehandWinner(fixture, true);
            PlayerFirstServedAndForehandWinner(fixture, true);
            PlayerFirstServedAndForehandWinner(fixture, true);
        }

        public static void AddAGameForPlayerTwo(MatchScoreViewModel fixture)
        {
            PlayerFirstServedAndForehandWinner(fixture, false);
            PlayerFirstServedAndForehandWinner(fixture, false);
            PlayerFirstServedAndForehandWinner(fixture, false);
            PlayerFirstServedAndForehandWinner(fixture, false);
        }

        public static void PlayerSecondServeInAndLostPoint(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeOutCommand.Execute(null);
                fixture.PlayerOneSecondServeInCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeOutCommand.Execute(null);
                fixture.PlayerTwoSecondServeInCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            }
        }

        public static void PlayerSecondServeInAndWonPoint(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeOutCommand.Execute(null);
                fixture.PlayerOneSecondServeInCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeOutCommand.Execute(null);
                fixture.PlayerTwoSecondServeInCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            }
        }

        public static void PlayerFirstServedAndForehandWinner(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeInCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeInCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            }
        }

        public static void PlayerFirstServeInAndLostPoint(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeInCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeInCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            }
        }

        public static void PlayerFirstServeOutAndDoubleFault(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeOutCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeOutCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);
            }
        }

        public static void PlayerFirstServeInAndUnforcedError(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeInCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "UnforcedForehandError").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeInCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "UnforcedForehandError").ActionCommand.Execute(null);

            }
        }        
        
        public static void PlayerFirstServeInAndForcedError(MatchScoreViewModel fixture, bool isPlayerOne)
        {
            if (isPlayerOne)
            {
                fixture.PlayerOneFirstServeInCommand.Execute(null);
                fixture.PlayerOneActions.First(x => x.Name == "ForcedError").ActionCommand.Execute(null);
            }
            else
            {
                fixture.PlayerTwoFirstServeInCommand.Execute(null);
                fixture.PlayerTwoActions.First(x => x.Name == "ForcedError").ActionCommand.Execute(null);

            }
        }

        public static TestBlobCache RegisterComponents()
        {
            EventListener verboseListener = new StorageFileEventListener("MyListenerVerbose1");
            EventListener informationListener = new StorageFileEventListener("MyListenerInformation1");

            verboseListener.EnableEvents(MatchStatsEventSource.Log, EventLevel.Error);
            RxApp.MutableResolver.RegisterConstant(MatchStatsEventSource.Log, typeof(MatchStatsEventSource));

            var blobCache = new TestBlobCache();
            RxApp.MutableResolver.Register(() => new MatchStatsApi(), typeof(IMatchStatsApi));
            RxApp.MutableResolver.RegisterConstant(blobCache, typeof(IBlobCache), "UserAccount");
            RxApp.MutableResolver.Register(() => new MatchStatsLogger(), typeof(ILogger));

            return blobCache;
        }

        public static MatchScoreViewModel BuildAMatchToScore()
        {
            var fixture = new MatchScoreViewModel();

            fixture.NewMatchControlViewModel = new NewMatchControlViewModel();
            fixture.NewMatchControlViewModel.PlayerOneFirstName = "William";
            fixture.NewMatchControlViewModel.PlayerOneLastName = "Dof";
            fixture.NewMatchControlViewModel.PlayerTwoFirstName = "Drake";
            fixture.NewMatchControlViewModel.PlayerTwoLastName = "Dufus";
            fixture.NewMatchControlViewModel.SelectedGrade = Grade.Grade3;
            fixture.NewMatchControlViewModel.SelectedAgeGroup = AgeGroup.U14;
            fixture.NewMatchControlViewModel.SelectedDeuceFormat = DeuceFormat.Normal;
            fixture.NewMatchControlViewModel.SelectedFinalSet = FinalSetFormats.Normal;
            fixture.NewMatchControlViewModel.SelectedPlayerOneRating = Rating.FiveOne;
            fixture.NewMatchControlViewModel.SelectedPlayerTwoRating = Rating.FiveOne;
            fixture.NewMatchControlViewModel.SelectedSetsFormat = SetsFormat.ShortSetToFour;
            fixture.NewMatchControlViewModel.TournamentName = "Sutton Open";
            return fixture;
        }
    }
}
