using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Akavache;
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

        public void SaveCredentials(string userName)
        {
            
        }

        public AppBootstrapper(IMutableDependencyResolver testResolver = null, IRoutingState router = null)
        {
            BlobCache.ApplicationName = "MatchStats";
            if (testResolver == null)
            {
                Router = router ?? new RoutingState();
                Resolver = testResolver ?? RxApp.MutableResolver;
                Resolver.Register(() => new MatchesPlayedView(), typeof (IViewFor<MatchesPlayedViewModel>),"FullScreenLandscape");
                Resolver.Register(() => new MatchesPlayedViewModel(this), typeof (IMatchesPlayedViewModel));

                Resolver.Register(() => new MatchScoreView(), typeof (IViewFor<MatchScoreViewModel>),"FullScreenLandscape");
                Resolver.Register(() => new MatchScoreViewModel(), typeof (MatchScoreViewModel));

                Resolver.RegisterConstant(this, typeof (IApplicationRootState));
                Resolver.RegisterConstant(this, typeof (IScreen));
                Resolver.RegisterConstant(this, typeof (ILoginMethods));
#if DEBUG
                var testBlobCache = new TestBlobCache();
                Resolver.RegisterConstant(testBlobCache, typeof (IBlobCache), "LOCALMACHINE");
                Resolver.RegisterConstant(testBlobCache, typeof (IBlobCache), "UserAccount");
                Resolver.RegisterConstant(testBlobCache, typeof (IBlobCache), "UserAccount");
                Resolver.RegisterConstant(testBlobCache, typeof (ISecureBlobCache));
#else
                resolver.RegisterConstant(BlobCache.Secure, typeof(ISecureBlobCache));
                resolver.RegisterConstant(BlobCache.LocalMachine, typeof(IBlobCache));
                resolver.RegisterConstant(BlobCache.UserAccount, typeof(IBlobCache));
#endif
                Resolver.RegisterConstant(new MainPage(), typeof (IViewFor), "InitialPage");
            }
            else
            {
                Resolver = testResolver;
                Router = router ?? testResolver.GetService<IRoutingState>();
            }

            Router.Navigate.Execute(Resolver.GetService<IMatchesPlayedViewModel>());
        }
    }
}