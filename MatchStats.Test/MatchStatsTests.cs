using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using MatchStats.Model;
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

            var ret = default(dynamic);
            using (var datareader = DataReader.FromBuffer(buffer))
            {
                ret = datareader.ReadString(buffer.Length);
            }
            ret = JsonConvert.DeserializeObject(ret);

        }
    }
}
