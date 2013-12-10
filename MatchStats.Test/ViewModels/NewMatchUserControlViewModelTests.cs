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
        public void ShouldFireSaveCommandIfAllRequiredFieldsAreSet()
        {
            var fixture = new NewMatchControlViewModel();
            fixture.PlayerOneFirstName = "Ademola";
            fixture.PlayerOneLastName = "Baruwa";

            fixture.PlayerTwoFirstName = "Winston";
            fixture.PlayerTwoLastName = "Babolat";

            fixture.SelectedAgeGroup = AgeGroup.U14;
            fixture.SelectedFinalSetFormat = FinalSetFormats.TenPointChampionShipTieBreak;

            fixture.SelectedSetsFormat = SetsFormat.ShortSetToFour;
            fixture.SelectedDueceFormat = DueceFormat.SuddenDeath;

            fixture.SelectedGrade = Grade.Grade5;

            Match matchMessage = null;
            MessageBus.Current.Listen<Match>().Subscribe(x =>
            {
                matchMessage = x;
            });

            fixture.SaveCommand.Execute(null);

            Assert.IsNotNull(matchMessage, "Message to save Match not received");
            //RxApp.DependencyResolver.GetService<IBlobCache>().InvalidateObject<List<Match>>("MatchList");
            //var matches = new List<Match>();
            //var matchListTestObservable =
            //    RxApp.DependencyResolver.GetService<IBlobCache>().GetObjectAsync<List<Match>>("MatchList");
            //matchListTestObservable.Subscribe(x =>
            //{
            //    if (x != null)
            //    {
            //        matches.AddRange(x);
            //    }
            //});

            //Assert.IsTrue(matches.Count.Equals(1));
        }

        [TestMethod]
        public void ShouldNotFireSaveCommandIfSetsFormatIsNotProvided()
        {
            var fixture = new NewMatchControlViewModel();
            fixture.PlayerOneFirstName = "Ademola";
            fixture.PlayerOneLastName = "Baruwa";

            fixture.PlayerTwoFirstName = "Winston";
            fixture.PlayerTwoLastName = "Babolat";

            Match matchMessage = null;
            MessageBus.Current.Listen<Match>().Subscribe(x =>
            {
                matchMessage = x;
            });

            Assert.IsFalse(fixture.SaveCommand.CanExecute(null), "ViewModel Validation failed, Message to save Match will be received, when it should not have been sent.");
            Assert.IsNull(matchMessage, "ViewModel Validation failed, Message to save Match received, when it should not have been sent."); 

 
        }
    }
}