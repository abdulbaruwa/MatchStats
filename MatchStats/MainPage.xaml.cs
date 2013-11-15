using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using MatchStats.ViewModels;
using ReactiveUI;

namespace MatchStats
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IViewFor<AppBootstrapper>
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.Router, x => x.Router.Router);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (AppBootstrapper), typeof (MainPage), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (AppBootstrapper) value; }
        }

        public AppBootstrapper ViewModel
        {
            get { return (AppBootstrapper) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
