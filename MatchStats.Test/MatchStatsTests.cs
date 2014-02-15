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
            Assert.AreEqual("67%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForFirstServe).ForMatchP1);
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
        public void ShouldCalculateOverallWinPercentForSecondServeForPlayerOne()
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

        [TestMethod]
        public void ShouldCalculateWinPercentForSecondServeForPlayerOneOnSecondSet()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);

            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("33%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForSecondServe).ForSecondSetP1);
        }

        [TestMethod]
        public void ShouldCalculateWinPercentForSecondServeForPlayerOneOnThirdTieBreakerSetSet()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);

            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, false);

            //0-1
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);

            //0-3
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            //2-3
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);

            // 2-5
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            //3-6
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);

            //3-8
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            //4-9
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndLostPoint(fixture, true);
            MatchScoreViewModelTestHelper.PlayerSecondServeInAndWonPoint(fixture, true);

            //4-10
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("57%", fixtureStats.Stats.First(x => x.StatNameType == StatName.WinPercentForSecondServe).ForThirdSetP1);
        }

        [TestMethod]
        public void ShouldCalculateWinner()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("2", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForMatchP1);
            Assert.AreEqual("5", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForMatchP2);
        }

        [TestMethod]
        public void ShouldCalculateWinnerInOtherSets()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, false);
            //0-1
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            //0-3
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            //2-3
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            // 2-5
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            //3-6
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            //3-8
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndLostPoint(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndLostPoint(fixture, false);

            //4-9
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            //4-10
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndLostPoint(fixture, false);


            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("26", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForMatchP1);
            Assert.AreEqual("21", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForMatchP2);
            Assert.AreEqual("16", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForFirstSetP1);
            Assert.AreEqual("0", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForFirstSetP2);
            Assert.AreEqual("0", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForSecondSetP1);
            Assert.AreEqual("16", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForSecondSetP2);
            Assert.AreEqual("10", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForThirdSetP1);
            Assert.AreEqual("5", fixtureStats.Stats.First(x => x.StatNameType == StatName.Winners).ForThirdSetP2);
        }

        [TestMethod]
        public void ShouldCalculateUnforcedErrors()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, true);

            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);

            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);

            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("2", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForMatchP1);
            Assert.AreEqual("3", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForMatchP2);
        }


        //Abdul TODO
        [TestMethod, Ignore]
        public void ShouldCalculateUnforcedErrorsForOtherSets()
        {
            var fixture = MatchScoreViewModelTestHelper.CreateNewMatchFixture(FinalSetFormats.TenPointChampionShipTieBreak);
            fixture.SetPlayerOneAsCurrentServerCommand.Execute(null);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, true);
            MatchScoreViewModelTestHelper.AddASetForPlayer(fixture, false);

            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            //0-3
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);

            //2-3
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            // 2-5
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);

            //3-6
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, true);

            //3-8
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndLostPoint(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndLostPoint(fixture, false);

            //4-9
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, true);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, true);

            //4-10
            MatchScoreViewModelTestHelper.PlayerFirstServedAndForehandWinner(fixture, false);
            MatchScoreViewModelTestHelper.PlayerFirstServeInAndUnforcedError(fixture, false);



            var fixtureStats = new MatchStatsViewModel();
            fixtureStats.CurrentMatch = fixture.CurrMatch;
            Assert.AreEqual("5", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForMatchP1);
            Assert.AreEqual("2", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForMatchP2);       
            
            Assert.AreEqual("2", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForFirstSetP1);
            Assert.AreEqual("3", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForFirstSetP2);

            Assert.AreEqual("3", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForSecondSetP1);
            Assert.AreEqual("3", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForSecondSetP2);

            Assert.AreEqual("3", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForThirdSetP1);
            Assert.AreEqual("3", fixtureStats.Stats.First(x => x.StatNameType == StatName.UnforcedErrors).ForThirdSetP2);
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
