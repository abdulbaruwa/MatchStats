using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akavache;
using MatchStats.Model;
using MatchStats.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer;
using ReactiveUI;
using Assert = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert;


namespace MatchStats.Test.ViewModels
{

    [TestClass]
    public class MatchesPlayedViewModelTests
    {
        [UITestMethod]
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

        public IMutableDependencyResolver RegisterTestResolver()
        {
            var resolver = RxApp.MutableResolver;
            //resolver.Register(() => new MatchesPlayedViewModel(new AppBootstrapper()), typeof(IMatchesPlayedViewModel));
            resolver.Register(() => new RoutingState(), typeof(IRoutingState));
            resolver.Register(() => new TestScreen(), typeof(IScreen));
            resolver.Register(() => new FakeUserSevice(), typeof(IUserService));
            var testBlobCache = new TestBlobCache();
            resolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "LOCALMACHINE");
            resolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "UserAccount");
            resolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "UserAccount");
            resolver.RegisterConstant(testBlobCache, typeof(ISecureBlobCache));

            return resolver;
//            Resolver = testResolver ?? RxApp.MutableResolver;

//            Resolver.Register(() => new MatchesPlayedView(), typeof(IViewFor<MatchesPlayedViewModel>), "FullScreenLandscape");
//            Resolver.Register(() => new MatchesPlayedViewModel(), typeof(IMatchesPlayedViewModel));

//            Resolver.Register(() => new MatchScoreView(), typeof(IViewFor<MatchScoreViewModel>), "FullScreenLandscape");
//            Resolver.Register(() => new MatchScoreViewModel(), typeof(MatchScoreViewModel));

//            Resolver.RegisterConstant(this, typeof(IApplicationRootState));
//            Resolver.RegisterConstant(this,typeof(IScreen));
//            Resolver.RegisterConstant(this, typeof(ILoginMethods));

//#if DEBUG
//            var testBlobCache = new TestBlobCache();
//            Resolver.RegisterConstant(testBlobCache,typeof(IBlobCache),"LOCALMACHINE");
//            Resolver.RegisterConstant(testBlobCache,typeof(IBlobCache),"UserAccount");
//            Resolver.RegisterConstant(testBlobCache,typeof(IBlobCache),"UserAccount");
//            Resolver.RegisterConstant(testBlobCache,typeof(ISecureBlobCache));
//#else
        }
    }

    public class TestScreen : IScreen
    {
        public IRoutingState Router { get; private set; }
    }
}
