using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Akavache;
using MatchStats.DesignTimeStuff;
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

    public class FakeMatchStatsApi : IMatchStatsApi
    {
        public void SaveMatchStats(IEnumerable<MyMatchStats> matchStats)
        {
            
        }

        public void SaveMatchStats(List<MyMatchStats> matchStats)
        {
            throw new NotImplementedException();
        }

        public void SaveMatch(Match match)
        {
            throw new NotImplementedException();
        }

        IObservable<List<MyMatchStats>> IMatchStatsApi.FetchMatchStats()
        {
            var outputList = new List<MyMatchStats>();
            outputList.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView());
            //create Cold stream observable from output list. We need the whole list as an observable out put not the items in the list
            IObservable<List<MyMatchStats>> observable = Observable.Create<List<MyMatchStats>>(o =>
            {
                o.OnNext(outputList);
                o.OnCompleted();
                return () => { };
            });

            return observable;
        }

        public IObservable<MyMatchStats> FetchMatchStats()
        {
            var outputList = new ReactiveList<MyMatchStats>();
            outputList.AddRange(new DummyDataBuilder().BuildMatchStatsForDesignTimeView());
            return outputList.ToObservable();
        }
    }

    public class TestScreen : IScreen
    {
        public IRoutingState Router { get; private set; }
    }
}
