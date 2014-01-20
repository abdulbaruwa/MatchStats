using System.Linq;
using Windows.UI.Xaml;
using MatchStats.ViewModels;
using ReactiveUI;

namespace MatchStats.Views
{
    public sealed partial class MatchStatsView : IViewFor<MatchStatsViewModel>
    {
        public MatchStatsView()
        {
            this.InitializeComponent();

        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MatchStatsViewModel) value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (MatchScoreViewModel), typeof (MatchScoreView), new PropertyMetadata(null));

        public MatchStatsViewModel ViewModel
        {
            get { return (MatchStatsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
