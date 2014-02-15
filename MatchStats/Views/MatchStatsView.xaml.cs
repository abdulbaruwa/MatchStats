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
            this.Bind(ViewModel, x => x.PlayerOneFullName, x => x.PlayerOneFullNameTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerOneFullName, x => x.PlayerOneNameUnderImage.Text);
            this.Bind(ViewModel, x => x.PlayerTwoFullName, x => x.PlayerTwoFullNameTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerTwoFullName, x => x.PlayerTwoNameUnderImage.Text);
            this.Bind(ViewModel, x => x.FirstSetDuration, x => x.FirstSetDurationTxtbox.Text);
            this.Bind(ViewModel, x => x.SecondSetDuration, x => x.SecondSetDurationTxtbox.Text);
            this.Bind(ViewModel, x => x.ThirdSetDuration, x => x.ThirdSetDurationTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerOneSetOneScore, x => x.PlayerOneSetOneScoreTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerOneSetTwoScore, x => x.PlayerOneSetTwoScoreTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerOneSetThreeScore, x => x.PlayerOneSetThreeScoreTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerTwoSetOneScore, x => x.PlayerTwoSetOneScoreTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerTwoSetTwoScore, x => x.PlayerTwoSetTwoScoreTxtbox.Text);
            this.Bind(ViewModel, x => x.PlayerTwoSetThreeScore, x => x.PlayerTwoSetThreeScoreTxtbox.Text);
            this.Bind(ViewModel, x => x.TotalPointsWonByPlayerOne, x => x.PlayerOneTotalPointsWon.Text);
            this.Bind(ViewModel, x => x.TotalPointsWonByPlayerTwo, x => x.PlayerTwoTotalPointsWon.Text);
            this.OneWayBind(ViewModel, x => x.HostScreen.Router.NavigateBack, x => x.BackButton.Command);
            this.Bind(ViewModel, x => x.Stats, x => x.StatsListView.ItemsSource);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MatchStatsViewModel) value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MatchStatsViewModel), typeof(MatchStatsView), new PropertyMetadata(null));

        public MatchStatsViewModel ViewModel
        {
            get { return (MatchStatsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
