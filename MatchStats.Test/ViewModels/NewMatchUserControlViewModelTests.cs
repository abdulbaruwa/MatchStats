using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
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
            fixture.SelectedDueceFormat = DueceFormat.SuddenDeath;
            fixture.SelectedGrade = Grade.Grade5;

            //Act
            fixture.SaveCommand.Execute(null);
           
            //Assert
            Assert.IsNotNull(fixture.SavedMatch, "Saved match was not set by Save Command execution");
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
            fixture.SelectedDueceFormat = DueceFormat.SuddenDeath;
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