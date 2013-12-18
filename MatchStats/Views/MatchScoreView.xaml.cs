using System.Linq;
using Windows.UI.Xaml;
using MatchStats.ViewModels;
using ReactiveUI;

namespace MatchStats.Views
{
    public sealed partial class MatchScoreView : IViewFor<MatchScoreViewModel>
    {
        public MatchScoreView()
        {
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.PlayerOneActions, x => x.PlayerOneCommands.ItemsSource);
            this.OneWayBind(ViewModel, x => x.PlayerTwoActions, x => x.PlayerTwoCommands.ItemsSource);
            this.Bind(ViewModel, x => x.PlayerOneCurrentGame, x => x.PlayerOneCurrentGame.Text);
            this.Bind(ViewModel, x => x.PlayerTwoCurrentGame, x => x.PlayerTwoCurrentGame.Text);
            this.Bind(ViewModel, x => x.PlayerOneFirstSet, x => x.PlayerOneFirstSet.Text);
            this.Bind(ViewModel, x => x.PlayerTwoFirstSet, x => x.PlayerTwoFirstSet.Text);
            this.Bind(ViewModel, x => x.PlayerOneSecondSet, x => x.PlayerOneSecondSet.Text);
            this.Bind(ViewModel, x => x.PlayerTwoSecondSet, x => x.PlayerTwoSecondSet.Text);
            this.Bind(ViewModel, x => x.PlayerOneThirdSet, x => x.PlayerOneThirdSet.Text);
            this.Bind(ViewModel, x => x.PlayerTwoThirdSet, x => x.PlayerTwoThirdSet.Text);
            this.OneWayBind(ViewModel, x => x.CurrentMatch.PlayerOne.FirstName, x => x.PlayerOnesName.Text);
            this.OneWayBind(ViewModel, x => x.CurrentMatch.PlayerTwo.FirstName, x => x.PlayerTwosName.Text);
            this.OneWayBind(ViewModel, x => x.HostScreen.Router.NavigateBack, x => x.backButton.Command);
            this.Bind(ViewModel, x => x.NewMatchControlViewModel, x => x.AddNewMatchControl.ViewModel);
            this.Bind(ViewModel, x => x.ShowHidePopup, x => x.AddMatchPoupup.IsOpen);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MatchScoreViewModel) value; }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (MatchScoreViewModel), typeof (MatchScoreView), new PropertyMetadata(null));

        public MatchScoreViewModel ViewModel
        {
            get { return (MatchScoreViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
