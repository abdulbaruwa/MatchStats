using System.Linq;
using MatchStats.Model;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MatchStats.Test.ViewModels.MatchScoreViewModelTests
{

    [TestClass]
    public class UndoActionsViewModel
    {
        [TestMethod]
        public void UndoActionShouldRemoveLastActionFromStat()
        {

            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            
            //Act
            fixture.UndoLastActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.AreEqual(StatDescription.FirstServeIn, fixture.CurrMatch.MatchStats.Last().Reason);
        }

        [TestMethod]
        public void WhenAtGamePointAnUndoActionShouldAlsoUndoTheLastPointAction()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsTrue(fixture.CurrMatch.MatchStats.Last().Reason == StatDescription.GamePoint);

            fixture.UndoLastActionCommand.Execute(null);
            
            Assert.AreEqual(StatDescription.FirstServeIn, fixture.CurrMatch.MatchStats.Last().Reason);
        }

        [TestMethod]
        public void WhenLastPointActionIsUnDoneShouldReflectChangeInTheScore()
        {

            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            // Two ace serves, the score is 30-0 to player One
            fixture.PlayerOneActions.First(x => x.Name == "AceServe").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "AceServe").ActionCommand.Execute(null);

            //Act
            fixture.UndoLastActionCommand.Execute(null);

            Assert.AreEqual("15", fixture.CurrMatch.GetPlayerOneCurrentScore());
        }

        [TestMethod]
        public void ShouldReverseGameWonWhenWinningPointForAGameIsReversed()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.AreEqual(1, fixture.CurrMatch.Sets.First().Games.Count);
        }

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenAPointIsUndone()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "AceServe").ActionCommand.Execute(null);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled);
        }        
        
        [TestMethod]
        public void ShouldDisableUndoCommandWhenCurrentServeIsNotSet()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            Assert.IsFalse(fixture.UndoLastActionCommand.CanExecute(null));
        }

        [TestMethod]
        public void ShouldDisableUndoCommandWhenThereIsNothingToUndo()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneActions.First(x => x.Name == "AceServe").ActionCommand.Execute(null);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.IsFalse(fixture.UndoLastActionCommand.CanExecute(null));
        }

        [TestMethod]
        public void ShouldChangeSetScoreIfWinningPointIsUndone()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true); 
            fixture.UndoLastActionCommand.Execute(null);
            Assert.AreEqual("3", fixture.PlayerOneFirstSet);
        }

        [TestMethod]
        public void ShouldChangeCurrentServerBackToPreviousServerIfWinningPointOfAGameIsUndone()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            //Current server should have changed here
            Assert.IsTrue(! fixture.CurrentServer.IsPlayerOne); 

            fixture.UndoLastActionCommand.Execute(null);
            Assert.IsTrue(fixture.CurrentServer.IsPlayerOne);
        }

        [TestMethod]
        public void ShouldChangeDisplayOfGameScoreForThePreviousGameIfTheWinningPointOfAGameIsUndone()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.AreEqual("40", fixture.PlayerOneCurrentGame);
        }

        [TestMethod]
        public void ShouldChangeDisplayOfGameScoreForThePreviousGameIfTheWinningPointIsOfAGameIsUndoneACoupleOfTimes()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);

            // First undo
            fixture.UndoLastActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.AreEqual("40", fixture.PlayerOneCurrentGame);
        }

        [TestMethod]
        public void ShouldChangeGameStatusFromGameOverIfTheWinningPointOfAGameIsUndone()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.AreEqual(Status.GamePoint, fixture.CurrMatch.CurrentGame().GameStatus.Status);
        }


        [TestMethod]
        public void ShouldEnableFirstAndSecondServeCommandsAfterAnUndoAction()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.UndoLastActionCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneFirstServeInCommand.CanExecute(null));
        }
    }
}
