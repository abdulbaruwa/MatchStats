using System.Linq;
using Akavache;
using MatchStats.Enums;
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
            return fixture;
        }

        public static MatchScoreViewModel CreateNewMatchFixture(FinalSetFormats finalSetFormat)
        {
            var fixture = CreateUnSavedMatchScoreViewModelFixture();
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
            //IReactiveCommand serveCommand = null;
            //if (fixture.CurrentServer.IsPlayerOne)
            //{
            //    serveCommand = fixture.PlayerOneFirstServeInCommand;
            //}

            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
        }

        public static void AddAGameForPlayerTwo(MatchScoreViewModel fixture)
        {
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
        }

        public static TestBlobCache RegisterComponents()
        {
            var blobCache = new TestBlobCache();
            RxApp.MutableResolver.Register(() => new MatchStatsApi(), typeof(IMatchStatsApi));
            RxApp.MutableResolver.RegisterConstant(blobCache, typeof(IBlobCache), "UserAccount");
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
