using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            ((ModernDependencyResolver)RxApp.DependencyResolver).Register(() => new AppBootstrapper(), typeof(IApplicationRootState));

            base.OnLaunched(e);
            var host = RxApp.DependencyResolver.GetService<ISuspensionHost>();
            if(host != null) host.SetupDefaultSuspendResume();
        }

    }
}
