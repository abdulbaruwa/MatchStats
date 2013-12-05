using Windows.UI.Xaml.Navigation;
using ReactiveUI;
using Windows.UI.Xaml;
using MatchStats.ViewModels;

namespace MatchStats.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchesPlayedView : IViewFor<MatchesPlayedViewModel>
    {

        public MatchesPlayedView()
        {
            this.InitializeComponent();
            ViewModel = new ViewModelLocator().MatchesPlayedViewModel;
            DataContext = ViewModel;
            this.BindCommand(ViewModel, x => x.AddMatch);
            this.Bind(ViewModel, x => x.ShowNewMatchPopup, x => x.AddMatchDialog.IsOpen);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MatchesPlayedViewModel) value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (MatchesPlayedViewModel), typeof (MatchesPlayedView), new PropertyMetadata(null));

        public MatchesPlayedViewModel ViewModel
        {
            get { return (MatchesPlayedViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
} 