using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using MatchStats.Model;
using MatchStats.Logging;
using MatchStats.ViewModels;
using System.Diagnostics.Tracing;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;

namespace MatchStats.Test.ViewModels
{
    [TestClass]
    public class UserProfileViewModelTests
    {
        [TestMethod]
        public async Task ShouldFetchDefaultUserOnInitialization()
        {
            RegisterComponents();
            var defaultPlayer = new Player()
            {FirstName = "FirstName", SurName = "Surname"};

            var blobCache = RxApp.MutableResolver.GetService<IBlobCache>("UserAccount");
            await blobCache.InsertObject("DefaultPlayer", defaultPlayer);

            //var xx = await RxApp.MutableResolver.GetService<IBlobCache>("UserAccount").GetObjectAsync<Player>("DefaultPlayer");
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

        [TestMethod]
        public void ShouldSaveUserAsDefaultPlayerWhenAllPlayerDetailsIsProvided()
        {
            RegisterComponents();
            var fixture = new UserProfileViewModel();
            var defaultPlayer = new Player() { FirstName = "FirstName", SurName = "Surname", Rating = "7.2"};

            fixture.DefaultPlayer = defaultPlayer;

            fixture.SaveDefaultPlayerCommand.Execute(null);
            RxApp.MutableResolver.GetService<IBlobCache>("UserAccount").GetObjectAsync<Player>("DefaultPlayer").Subscribe(
                Assert.IsNotNull, ex => Assert.Fail("Default Player assert fails"));
        }

        [TestMethod]
        public void ShouldNotSaveUserAsDefaultIfAllPlayerDetailsIsNotProvided()
        {
            RegisterComponents();
            var fixture = new UserProfileViewModel();
            var defaultPlayer = new Player() { FirstName = "FirstName", Rating = "7.2" };

            fixture.DefaultPlayer = defaultPlayer;

            Assert.IsFalse(fixture.SaveDefaultPlayerCommand.CanExecute(null));

            //fixture.SaveDefaultPlayerCommand.Execute(null);

            //RxApp.MutableResolver.GetService<IBlobCache>("UserAccount").GetObjectAsync<Player>("DefaultPlayer").Subscribe(
            //    Assert.IsNull, ex => Assert.Fail("Default Player assert fails"));
 
        }

    }
}
