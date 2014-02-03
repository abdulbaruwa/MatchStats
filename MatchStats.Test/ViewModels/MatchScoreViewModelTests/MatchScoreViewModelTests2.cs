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
    }
}
