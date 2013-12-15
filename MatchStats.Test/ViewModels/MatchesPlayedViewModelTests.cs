using System.Linq;
using Akavache;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;
using Assert = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert;


namespace MatchStats.Test.ViewModels
{

    [TestClass]
    public class MatchesPlayedViewModelTests
    {
        [TestMethod]
        public void ShouldGetAndSaveUserCredentialToDisk()
        {
            //On instantiation the viewmodel should get the User and save to Cache
            //if not already in the cache.

            var testResolver = RxApp.DependencyResolver;
            var fixture = new MatchesPlayedViewModel(new AppBootstrapper(RegisterTestResolver()));

            var testblobCache = testResolver.GetService<ISecureBlobCache>();
            var tokenName = testblobCache.GetAllKeys().ToList().First();

            Assert.AreEqual(tokenName,"Token", "Credentials token not found");
        }

        [TestMethod]
        public void ShouldRetrieveSavedMatchStatsOnViewModelLoad()
        {
            var fixture = new MatchesPlayedViewModel(new AppBootstrapper(RegisterTestResolver()));
            Assert.IsNotNull(fixture.MyMatchStats, "Match Stats not initialised");
        }

        private IMutableDependencyResolver RegisterTestResolver()
        {
            var resolver = RxApp.MutableResolver;
            //resolver.Register(() => new MatchesPlayedViewModel(new AppBootstrapper()), typeof(IMatchesPlayedViewModel));
            resolver.Register(() => new RoutingState(), typeof(IRoutingState));
            resolver.Register(() => new TestScreen(), typeof(IScreen));
            resolver.Register(() => new FakeUserSevice(), typeof(IUserService));
            resolver.Register(() => new FakeMatchStatsApi(), typeof(IMatchStatsApi));
            var testBlobCache = new TestBlobCache();
            resolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "LOCALMACHINE");
            resolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "UserAccount");
            resolver.RegisterConstant(testBlobCache, typeof(ISecureBlobCache));

            return resolver;
        }
    }

    public class TestScreen : IScreen
    {
        public IRoutingState Router { get; private set; }
    }
}
