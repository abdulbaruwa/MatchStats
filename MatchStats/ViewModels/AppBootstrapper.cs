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
        void SaveCredentials(string userName);
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

        public void SaveCredentials(string userName)
        {
            
        }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            var resolver = RxApp.MutableResolver;

            resolver.Register(() => new MatchesPlayedView(), typeof(IViewFor<MatchesPlayedViewModel>), "FullScreenLandscape");
            resolver.Register(() => new MatchesPlayedViewModel(), typeof(IMatchesPlayedViewModel));

            resolver.Register(() => new MatchScoreView(), typeof(IViewFor<MatchScoreViewModel>), "FullScreenLandscape");
            resolver.Register(() => new MatchScoreViewModel(), typeof(MatchScoreViewModel));

            resolver.RegisterConstant(this, typeof(IApplicationRootState));
            resolver.RegisterConstant(this,typeof(IScreen));
            resolver.RegisterConstant(this, typeof(ILoginMethods));

#if DEBUG
            var testBlobCache = new TestBlobCache();
            resolver.RegisterConstant(testBlobCache,typeof(IBlobCache),"LOCALMACHINE");
            resolver.RegisterConstant(testBlobCache,typeof(IBlobCache),"UserAccount");
            resolver.RegisterConstant(testBlobCache,typeof(IBlobCache),"UserAccount");
            resolver.RegisterConstant(testBlobCache,typeof(ISecureBlobCache));
#else
            resolver.RegisterConstant(BlobCache.Secure, typeof(ISecureBlobCache));
            resolver.RegisterConstant(BlobCache.LocalMachine, typeof(IBlobCache));
            resolver.RegisterConstant(BlobCache.UserAccount, typeof(IBlobCache));
#endif
            resolver.RegisterConstant(new MainPage(), typeof(IViewFor), "InitialPage");
            Router.Navigate.Execute(new MatchesPlayedViewModel(this));
        }
    }
}