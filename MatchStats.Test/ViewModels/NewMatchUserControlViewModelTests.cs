using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using MatchStats.Enums;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;

namespace MatchStats.Test.ViewModels
{
    [TestClass]
    public class NewMatchUserControlViewModelTests
    {
        [TestMethod]
        public void ShouldUpdateSaveMatchIfSaveCommandIsFired()
        {
            var fixture = new NewMatchControlViewModel();

            fixture.PlayerOneFirstName = "Ademola";
            fixture.PlayerOneLastName = "Baruwa";

            fixture.PlayerTwoFirstName = "Winston";
            fixture.PlayerTwoLastName = "Babolat";

            fixture.SelectedAgeGroup = AgeGroup.U14;
            fixture.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;

            fixture.SelectedSetsFormat = SetsFormat.ShortSetToFour;
            fixture.SelectedDeuceFormat = DeuceFormat.SuddenDeath;
            fixture.SelectedGrade = Grade.Grade5;

            //Act
            fixture.SaveCommand.Execute(null);
           
            //Assert
            Assert.IsNotNull(fixture.SavedMatch, "Saved match was not set by Save Command execution");
        }

        [TestMethod]
        public async Task ShouldUseDefaultPlayerIfUseDefaultPlayerFlagIsSet()
        {

            var playerOne = new Player() {FirstName = "FirstName", SurName = "Surname", Rating = "SevenTwo"};
            var fixture = new NewMatchControlViewModel();
            var testBlobCache = new TestBlobCache();
            RxApp.MutableResolver.Register(() => testBlobCache, typeof (IBlobCache), "UserAccount");
            await RxApp.MutableResolver.GetService<IBlobCache>("UserAccount").InsertObject("DefaultPlayer", playerOne);
            fixture.UseDefaultPlayer = true;
            fixture.TournamentName = "TestTournament";
            fixture.PlayerTwoFirstName = "Winston";
            fixture.PlayerTwoLastName = "Babolat";

            fixture.SelectedAgeGroup = AgeGroup.U14;
            fixture.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;

            fixture.SelectedSetsFormat = SetsFormat.ShortSetToFour;
            fixture.SelectedDeuceFormat = DeuceFormat.SuddenDeath;
            fixture.SelectedGrade = Grade.Grade5;

            Assert.IsTrue(fixture.SaveCommand.CanExecute(null), "Parameters for Save Command not set");

            fixture.SaveCommand.Execute(null);
           
            //Assert
            Assert.IsNotNull(fixture.SavedMatch, "Saved match was not set by Save Command execution");
            Assert.IsNotNull(fixture.SavedMatch.PlayerOne, "Player one was not set by Save Command execution");
        }

        [TestMethod]
        public void ShouldEnableSaveCommandIfRequiredFieldsAreUpdated()
        {
            var fixture = new NewMatchControlViewModel();
            fixture.PlayerOneFirstName = "Ademola";
            fixture.PlayerOneLastName = "Baruwa";

            fixture.PlayerTwoFirstName = "Winston";
            fixture.PlayerTwoLastName = "Babolat";

            fixture.TournamentName = "Sutton Open";
            fixture.SelectedAgeGroup = AgeGroup.U14;
            fixture.SelectedFinalSet = FinalSetFormats.TenPointChampionShipTieBreak;
            fixture.SelectedSetsFormat = SetsFormat.ShortSetToFour;
            fixture.SelectedDeuceFormat = DeuceFormat.SuddenDeath;
            fixture.SelectedGrade = Grade.Grade5;

            //Act & Assert
            Assert.IsTrue(fixture.SaveCommand.CanExecute(null), "Parameters for Save Command not set");

        }

        [TestMethod]
        public void ShouldNotFireSaveCommandIfSetsFormatIsNotProvided()
        {
            var fixture = new NewMatchControlViewModel();
            fixture.PlayerOneFirstName = "Ademola";
            fixture.PlayerOneLastName = "Baruwa";

            fixture.PlayerTwoFirstName = "Winston";
            fixture.PlayerTwoLastName = "Babolat";

            Assert.IsFalse(fixture.SaveCommand.CanExecute(null), "ViewModel SaveCommand CanExecute validation failed");
        }
    }
}