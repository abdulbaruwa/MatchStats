using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MatchStats.Test.ViewModels.MatchScoreViewModelTests
{
    [TestClass]
    public class ActionsViewModelTests
    {

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenAPlayerIsServing()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            Assert.IsTrue(fixture.PlayerOneActions.Any(x => x.Name == "AceServe" && x.IsEnabled));
        }

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenAPlayerTwoIsServing()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);
            Assert.IsTrue(fixture.PlayerTwoActions.Any(x => x.Name == "AceServe" && x.IsEnabled));
        }

        [TestMethod]
        public void ShouldDisableAceActionCommandWhenPlayerOneServesFirstServeIn()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled);
        }

        [TestMethod]
        public void ShouldDisableAceActionCommandWhenPlayerTwoServesFirstServeIn()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerTwoActions.First(x => x.Name == "AceServe").IsEnabled);
        }

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenPlayerOneServesTheFirstServeOut()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled);
        }

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenScoreIsGamePointAndPlayerOneServesTheFirstServeOut()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled);
        }

        [TestMethod]
        public void ShouldEnableAceActionCommandWhenPlayerTwoServesTheFirstServeOut()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoFirstServeOutCommand.Execute(null);

            Assert.IsTrue(fixture.PlayerTwoActions.First(x => x.Name == "AceServe").IsEnabled);
        }


        [TestMethod]
        public void ShouldDisableAceActionCommandWhenPlayerOneServesSecondServeIn()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneSecondServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerOneActions.First(x => x.Name == "AceServe").IsEnabled);
        }


        [TestMethod]
        public void ShouldDisableAceActionCommandWhenPlayerTwoServesSecondServeIn()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();
            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoFirstServeOutCommand.Execute(null);
            fixture.PlayerTwoSecondServeInCommand.Execute(null);

            Assert.IsFalse(fixture.PlayerTwoActions.First(x => x.Name == "AceServe").IsEnabled);
        }
    }
}
