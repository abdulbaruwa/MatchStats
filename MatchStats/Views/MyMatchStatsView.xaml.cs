using ReactiveUI;
using Windows.UI.Xaml;
using MatchStats.ViewModels;

namespace MatchStats.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyMatchStatsView : IViewFor<MyMatchStatsViewModel>
    {
        public MyMatchStatsView()
        {
            this.InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MyMatchStatsViewModel) value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (MyMatchStatsViewModel), typeof (MyMatchStatsView), new PropertyMetadata(null));

        public MyMatchStatsViewModel ViewModel
        {
            get { return (MyMatchStatsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
