using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using MatchStats.DesignTimeStuff;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MatchStats.Test
{
    [TestClass] public class MatchStatsTests 
    {
        [TestMethod]
        public async Task ShouldCalculatePercentageFirstServesInAndDoubleFaultsForPlayerOne()
        {
            var matchs = await SerializeMatchStatsFromJsonData();

            var matchstat = new MatchStat();
            var player1 = new PlayerBuilder().WithFirstNameSecondName("Ade", "Wilson").IsPlayerOne(true).Build();
            var player2 = new PlayerBuilder().WithFirstNameSecondName("Luke", "Watson").IsPlayerOne(true).Build();
            var setId = matchs.First().Score.Sets.First().SetId;
            var matchStatsList = new List<MatchStat>();
            //15-0
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn, SetId = setId});

            //15-15
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeOut, SetId = setId});
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.SecondServeIn, SetId = setId });

            //30-15
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn, SetId = setId });
            //30-30
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn, SetId = setId });
            //40-30
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.PointWon, Reason = StatDescription.FirstServeAce, SetId = setId });
            //40-40
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeOut, SetId = setId });
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.PointLost, Reason = StatDescription.DoubleFault, SetId = setId });
            //Adv
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn, SetId = setId });
            //Game
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeOut, SetId = setId });
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.SecondServeIn, SetId = setId });

            //var match = new Match {MatchStats = matchStatsList, PlayerOne = player1, PlayerTwo = player2};
            var match = matchs.FirstOrDefault();

            match.MatchStats.Clear();
            match.MatchStats.AddRange(matchStatsList);
            var fixture = new MatchStatsViewModel();
            fixture.CurrentMatch = match;
            Assert.IsNotNull(fixture.Stats);
            Assert.IsTrue(fixture.Stats.Count > 0);
            Assert.AreEqual("62%", fixture.Stats.First(x => x.StatNameType == StatName.FirstServePercentage).ForMatchP1);
            Assert.IsNotNull(fixture.Stats.FirstOrDefault(x => x.StatNameType == StatName.DoubleFaults));
            Assert.AreEqual("1", fixture.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForMatchP1);
            Assert.AreEqual("0", fixture.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForMatchP2);
            Assert.AreEqual("1", fixture.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForFirstSetP1);
            Assert.AreEqual("0", fixture.Stats.First(x => x.StatNameType == StatName.DoubleFaults).ForFirstSetP2);
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
