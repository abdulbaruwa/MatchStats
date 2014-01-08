using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.Serialization;
using Akavache;
using MatchStats.Controls;
using MatchStats.DesignTimeStuff;
using MatchStats.Model;
using MatchStats.Views;
using ReactiveUI;
using ReactiveUI.Mobile;

namespace MatchStats.ViewModels
{
    public interface ILoginMethods : IApplicationRootState
    {
       void SaveCredentials(string userName = null);
    }

    [DataContract]
    public class AppBootstrapper : ReactiveObject, ILoginMethods
    {
        [DataMember]private RoutingState _router;
        public IRoutingState Router
        {
            get { return _router; }
            set { _router = (RoutingState) value; }
        }

        public IMutableDependencyResolver Resolver { get; protected set; }

        public void SaveCredentials(string userName = null)
        {
            var username = userName ?? Token;
            var blobCache = RxApp.MutableResolver.GetService<ISecureBlobCache>();
            blobCache.InsertObject("Token", username);
        }

        private string _userloginSaved;
        public string UserLoginSaved
        {
            get { return _userloginSaved; }
            set { this.RaiseAndSetIfChanged(ref _userloginSaved, value); }
        }

        public AppBootstrapper(IMutableDependencyResolver testResolver = null, IRoutingState router = null)
        {
            BlobCache.ApplicationName = "MatchStats";
            RegiserResolver(testResolver, router);
 
            GetCredentials().Subscribe(
                x =>
                {
                    Token = x;
                    Router.Navigate.Execute(new MatchesPlayedViewModel(this));
                }, ex =>
                {
                    this.Log().WarnException("Failed to get the logged in user", ex);
                    Router.Navigate.Execute(Resolver.GetService<IMatchesPlayedViewModel>());
                });
        }

        public IObservable<string> GetCredentials()
        {
            return RxApp.DependencyResolver.GetService<IUserService>().GetCurrentUserAsync().ToObservable();
        }

        private string _token;
        public string Token
        {
            get { return _token; }
            set { this.RaiseAndSetIfChanged(ref _token, value); }
        }

        private void RegiserResolver(IMutableDependencyResolver testResolver, IRoutingState router)
        {
            if (testResolver == null)
            {
                Router = router ?? new RoutingState();
                Resolver = testResolver ?? RxApp.MutableResolver;
                Resolver.RegisterConstant(this, typeof(IApplicationRootState));
                Resolver.RegisterConstant(this, typeof(IScreen));
                Resolver.RegisterConstant(this, typeof(ILoginMethods));

                Resolver.Register(() => new MatchStatsApi(), typeof(IMatchStatsApi));

                Resolver.Register(() => new MatchesPlayedViewModel(this), typeof(MatchesPlayedViewModel));
                Resolver.Register(() => new MatchesPlayedView(), typeof (IViewFor<MatchesPlayedViewModel>),"FullScreenLandscape");

                Resolver.Register(() => new NewMatchControlViewModel(), typeof(NewMatchControlViewModel));
                Resolver.Register(() => new NewMatchUserControl(), typeof(IViewFor<NewMatchControlViewModel>));

                Resolver.Register(() => new MatchScoreViewModel(), typeof(MatchScoreViewModel));
                Resolver.Register(() => new MatchScoreView(), typeof (IViewFor<MatchScoreViewModel>), "FullScreenLandscape");
                Resolver.Register(() => new UserService(), typeof (IUserService));

               

#if DEBUG
                var testBlobCache = new TestBlobCache();
                Resolver.RegisterConstant(testBlobCache, typeof (IBlobCache), "LOCALMACHINE");
                Resolver.RegisterConstant(testBlobCache, typeof (IBlobCache), "UserAccount");
                Resolver.RegisterConstant(testBlobCache, typeof (ISecureBlobCache));
                var matchStatsApi = Resolver.GetService<IMatchStatsApi>();
                matchStatsApi.SaveMatchStats(new DummyDataBuilder().BuildMatchStatsForDesignTimeView().ToList());
#else
                resolver.RegisterConstant(BlobCache.Secure, typeof(ISecureBlobCache));
                resolver.RegisterConstant(BlobCache.LocalMachine, typeof(IBlobCache));
                resolver.RegisterConstant(BlobCache.UserAccount, typeof(IBlobCache));
#endif
                Resolver.RegisterConstant(this, typeof(AppBootstrapper));
                Resolver.RegisterConstant(new MainPage(), typeof (IViewFor), "InitialPage");
            }
            else
            {
                Resolver = testResolver;
                Router = router ?? testResolver.GetService<IRoutingState>();
                var asd = testResolver.GetService<ISecureBlobCache>();
            }
        }
    }
}