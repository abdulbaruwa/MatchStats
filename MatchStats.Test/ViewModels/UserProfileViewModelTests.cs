using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akavache;
using MatchStats.Logging;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;

namespace MatchStats.Test.ViewModels
{
    [TestClass]
    public class UserProfileViewModelTests
    {
        [TestMethod]
        public void ShouldFetchDefaultUserOnInitialization()
        {
            RegisterComponents();
            var defaultPlayer = new Player()
            {FirstName = "FirstName", SurName = "Surname"};

            var blobCache = RxApp.MutableResolver.GetService<IBlobCache>("UserAccount");
            blobCache.InsertObject("DefaultPlayer", defaultPlayer);
            var fixture = new UserProfileViewModel();
            Assert.IsNotNull(fixture.DefaultPlayer);
        }

        public static TestBlobCache RegisterComponents()
        {
            EventListener verboseListener = new StorageFileEventListener("MyListenerVerbose1");
            EventListener informationListener = new StorageFileEventListener("MyListenerInformation1");

            verboseListener.EnableEvents(MatchStatsEventSource.Log, EventLevel.Error);
            RxApp.MutableResolver.RegisterConstant(MatchStatsEventSource.Log, typeof(MatchStatsEventSource));

            var blobCache = new TestBlobCache();
            RxApp.MutableResolver.Register(() => new MatchStatsApi(), typeof(IMatchStatsApi));
            RxApp.MutableResolver.RegisterConstant(blobCache, typeof(IBlobCache), "UserAccount");
            RxApp.MutableResolver.Register(() => new MatchStatsLogger(), typeof(ILogger));

            return blobCache;
        }

    }
}
