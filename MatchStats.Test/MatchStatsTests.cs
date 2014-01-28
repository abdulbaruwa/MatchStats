using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;

namespace MatchStats.Test
{
    [TestClass] public class MatchStatsTests 
    {
        [TestMethod]
        public async Task ShouldCalculatePercentageFirstServesInForPlayerOne()
        {
            var path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + @"\TestData\MyMatchStats.txt";
            var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            var buffer = await FileIO.ReadBufferAsync(storageFile);

            string ret = "";
            using (var datareader = DataReader.FromBuffer(buffer))
            {
                ret = datareader.ReadString(buffer.Length);
            }
            var result = JsonConvert.DeserializeObject(ret);

        }
    }
}
