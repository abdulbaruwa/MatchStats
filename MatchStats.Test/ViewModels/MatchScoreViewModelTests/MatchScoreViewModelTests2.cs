using System.Linq;
using MatchStats.Model;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MatchStats.Test.ViewModels.MatchScoreViewModelTests
{
    [TestClass]
    public class MatchScoreViewModelTests2
    {
        [TestMethod]
        public void ShouldAddGamePointSituationToMatchStatOnGamePoint()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);
            Assert.AreEqual(MatchSituationType.GamePoint, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);
        }        
        
        [TestMethod]
        public void ShouldAddBreakPointSituationToMatchStatOnBreakPoint()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);
            Assert.AreEqual(MatchSituationType.BreakPoint, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);
        }
         
        [TestMethod]
        public void ShouldAddGamePointWonSituationToMatchStatOnGamePointWon()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);
            Assert.AreEqual(MatchSituationType.GamePointWon, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);
        }

        [TestMethod]
        public void ShouldAddBreakPointWonSituationToMatchStatOnBreakPointWon()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeOutCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);
            Assert.AreEqual(MatchSituationType.BreakPointWon, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);
        }


        [TestMethod]
        public void ShouldAddMatchPointSituationToMatchStatOnMatchPoint()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            fixture.CurrMatch.Score.Sets.First().Winner = fixture.CurrMatch.PlayerOne;

            fixture.SetPlayerTwoAsCurrentServerCommand.Execute(null);

            fixture.PlayerTwoFirstServeOutCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerTwoFirstServeOutCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            fixture.PlayerTwoFirstServeOutCommand.Execute(null);
            fixture.PlayerTwoActions.First(x => x.Name == "DoubleFault").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);

            Assert.AreEqual(MatchSituationType.MatchPoint, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);

        }


        [TestMethod]
        public void ShouldAddMatchPointSituationToMatchStatOnMatchPointWithWinningPlayerServingForTheMatch()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            fixture.CurrMatch.Score.Sets.First().Winner = fixture.CurrMatch.PlayerOne;

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);

            Assert.AreEqual(MatchSituationType.MatchPoint, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);
        }


        [TestMethod]
        public void ShouldAddMatchPointSituationToMatchStatOnMatchPointWithBothScoreInSetsAtOneAllAndPlayerOneServingForTheMatch()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture();

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, false);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);
            fixture.CurrMatch.Score.Sets.First().Winner = fixture.CurrMatch.PlayerOne;

            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            fixture.PlayerOneFirstServeInCommand.Execute(null);
            fixture.PlayerOneActions.First(x => x.Name == "ForeHandWinner").ActionCommand.Execute(null);

            Assert.IsNotNull(fixture.CurrMatch.MatchStats.LastOrDefault());
            Assert.IsTrue(fixture.CurrMatch.MatchStats.LastOrDefault().MatchSituations.Count > 0);

            Assert.AreEqual(MatchSituationType.MatchPoint, fixture.CurrMatch.MatchStats.Last().MatchSituations.Last().MatchSituationType);
        }

    }
}
