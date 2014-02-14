using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using MatchStats.DesignTimeStuff;
using MatchStats.Enums;
using MatchStats.Model;
using MatchStats.Test.ViewModels.MatchScoreViewModelTests;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatchStats.Test
{
    [TestClass] public class MatchStatsTests 
    {
        [TestMethod]
        public void ShouldCalculateFirstServePercentageForPlayerOne()
        {
            var fixture =  MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.PlayerFirstServeOutAndDoubleFault(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServeOutAndDoubleFault(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("67%", fixtureStats.Stats.First(x => x.StatNameType == StatName.FirstServePercentage).ForMatchP1);
            Assert.AreEqual("2", fixtureStats.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForMatchP1);
            Assert.AreEqual("0", fixtureStats.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForMatchP2);
            Assert.AreEqual("2", fixtureStats.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForFirstSetP1);
            Assert.AreEqual("0", fixtureStats.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForFirstSetP2);
        }


        [TestMethod]
        public void ShouldCalculateWinPercentForFirstServeForPlayerOne()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.PlayerFirstServeOutAndDoubleFault(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("50%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForFirstServe).ForMatchP1);
        }

        [TestMethod]
        public void ShouldCalculateWinPercentForFirstServeForPlayerOneIFirstSet()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, false);
            MatchScoreViewModelTestHelper.AddAGameForPlayerOne(fixture);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("50%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForFirstServe).ForFirstSetP1);
            Assert.AreEqual("50%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForFirstServe).ForSecondSetP2);
        }


        [TestMethod]
        public void ShouldCalculateWinPercentForSecondServeForPlayerOne()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("67%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForSecondServe).ForMatchP1);
        }


        private static async Task<List<Match>> SerializeMatchStatsFromJsonData()
        {
            var path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + @"\TestData\MyMatchStats.txt";
            var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            var buffer = await FileIO.ReadBufferAsync(storageFile);
            var ret = "";
            using (var datareader = DataReader.FromBuffer(buffer))
            {
                ret = datareader.ReadString(buffer.Length);
            }

            var data = JObject.Parse(ret)["Value"];
            return JsonConvert.DeserializeObject<List<Match>>(data.ToString());
        }
    }
}
