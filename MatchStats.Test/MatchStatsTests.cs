using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        public async Task ShouldCalculatePercentageFirstServesInForPlayerOne()
        {
            var matchStats = await SerializeMatchStatsFromJsonData();
            var matchstat = new MatchStat();
            var player1 = new PlayerBuilder().WithFirstNameSecondName("Ade", "Wilson").IsPlayerOne(true).Build();
            var player2 = new PlayerBuilder().WithFirstNameSecondName("Luke", "Watson").IsPlayerOne(true).Build();
            var matchStatsList = new List<MatchStat>();
            //15-0
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn});

            //15-15
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeOut});
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.SecondServeIn});

            //30-15
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn});
            //30-30
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn});
            //40-30
            matchStatsList.Add(new MatchStat(){Player = player1, PointWonLostOrNone = PointWonLostOrNone.PointWon, Reason = StatDescription.FirstServeAce});
            //40-40
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeOut });
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.PointLost, Reason = StatDescription.DoubleFault });
            //Adv
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeIn });
            //Game
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.FirstServeOut });
            matchStatsList.Add(new MatchStat() { Player = player1, PointWonLostOrNone = PointWonLostOrNone.NotAPoint, Reason = StatDescription.SecondServeIn });

            var match = new Match {MatchStats = matchStatsList, PlayerOne = player1, PlayerTwo = player2};
            var fixture = new MatchStatsViewModel();
            fixture.CurrentMatch = match;

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
