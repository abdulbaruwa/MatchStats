using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using MatchStats.Model;
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
