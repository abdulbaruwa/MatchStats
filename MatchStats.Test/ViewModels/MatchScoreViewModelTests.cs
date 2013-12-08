using MatchStats.Enums;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MatchStats.Test.ViewModels
{
    [TestClass]
    public class MatchScoreViewModelTests
    {
        [TestMethod]
        public void ShouldSaveGiveGameToListOfGames()
        {
            var fixture = new MatchScoreViewModel();
            
            fixture.NewMatchControlViewModel = new NewMatchControlViewModel();

        }
    }
}