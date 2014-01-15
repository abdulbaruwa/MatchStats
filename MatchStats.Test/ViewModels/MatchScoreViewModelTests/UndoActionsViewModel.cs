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

            Assert.AreEqual(1, fixture.CurrMatch.Score.Sets.First().Games.Count);
        }
    }
}
