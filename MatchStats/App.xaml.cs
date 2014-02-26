using System.Diagnostics.Tracing;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Akavache;
using MatchStats.DesignTimeStuff;
using MatchStats.Logging;
using MatchStats.Model;
using MatchStats.Observables;
using MatchStats.ViewModels;
using ReactiveUI;
using ReactiveUI.Mobile;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace MatchStats
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : AutoSuspendApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            EventListener verboseListener = new StorageFileEventListener("MyListenerVerbose1");
            EventListener informationListener = new StorageFileEventListener("MyListenerInformation1");

            verboseListener.EnableEvents(MatchStatsEventSource.Log, EventLevel.Verbose);
            informationListener.EnableEvents(MatchStatsEventSource.Log, EventLevel.Informational);

            BlobCache.ApplicationName = "MatchStats";

#if DEBUG
            // TODO Doing this to enable testing of the SQLITE Akavach otherwise falling to TestBlobCache which is in memory 
            //var testBlobCache = new TestBlobCache();
            //RxApp.MutableResolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "LOCALMACHINE");
            //RxApp.MutableResolver.RegisterConstant(testBlobCache, typeof(IBlobCache), "UserAccount");
            //RxApp.MutableResolver.RegisterConstant(testBlobCache, typeof(ISecureBlobCache));

            RxApp.MutableResolver.RegisterConstant(BlobCache.Secure, typeof(ISecureBlobCache));
            RxApp.MutableResolver.RegisterConstant(BlobCache.LocalMachine, typeof(IBlobCache));
            RxApp.MutableResolver.RegisterConstant(BlobCache.UserAccount, typeof(IBlobCache));
            RxApp.MutableResolver.Register(() => new SchedulerProvider(), typeof(ISchedulerProvider));


#else
            RxApp.MutableResolver.Register(() => new SchedulerProvider(), typeof(ISchedulerProvider));
            RxApp.MutableResolver.RegisterConstant(BlobCache.Secure, typeof(ISecureBlobCache));
            RxApp.MutableResolver.RegisterConstant(BlobCache.LocalMachine, typeof(IBlobCache));
            RxApp.MutableResolver.RegisterConstant(BlobCache.UserAccount, typeof(IBlobCache));
#endif

            ((ModernDependencyResolver)RxApp.DependencyResolver).RegisterConstant(MatchStatsEventSource.Log, typeof(MatchStatsEventSource));
            ((ModernDependencyResolver)RxApp.DependencyResolver).Register(() => new MatchStatsLogger(), typeof(ILogger));
            ((ModernDependencyResolver)RxApp.DependencyResolver).Register(() => new AppBootstrapper(), typeof(IApplicationRootState));

            base.OnLaunched(e);
            var host = RxApp.DependencyResolver.GetService<ISuspensionHost>();
            if(host != null) host.SetupDefaultSuspendResume();
        }

    }
}
