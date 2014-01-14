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
    }
}
