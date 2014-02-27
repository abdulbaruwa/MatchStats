using System.Linq;
using System.Xml.Linq;
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
            this.OneWayBind(ViewModel, x => x.CurrMatch.PlayerOne.FirstName, x => x.PlayerOnesName.Text);
            this.OneWayBind(ViewModel, x => x.CurrMatch.PlayerTwo.FirstName, x => x.PlayerTwosName.Text);
            this.OneWayBind(ViewModel, x => x.HostScreen.Router.NavigateBack, x => x.backButton.Command);
            this.Bind(ViewModel, x => x.NewMatchControlViewModel, x => x.AddNewMatchControl.ViewModel);
            this.Bind(ViewModel, x => x.ShowHidePopup, x => x.AddMatchPoupup.IsOpen);
            this.Bind(ViewModel, x => x.PlayerOneIsServing, x => x.PlayerOneIsServing.IsChecked);
            this.Bind(ViewModel, x => x.PlayerTwoIsServing, x => x.PlayerTwoIsServing.IsChecked);
            this.Bind(ViewModel, x => x.CurrMatch.Tournament.TournamentName, x => x.TournamentTitle.Text);
            this.Bind(ViewModel, x => x.CurrMatch.PlayerOne.FullName, x => x.PlayerOneNameUnderImage.Text);
            this.Bind(ViewModel, x => x.CurrMatch.PlayerTwo.FullName, x => x.PlayerTwoNameUnderImage.Text);
            this.Bind(ViewModel, x => x.StartPause, x => x.StartPauseMatchActionCommand.Content);
            this.Bind(ViewModel, x => x.Timing, x => x.MatchTiming.Text);
            this.Bind(ViewModel, x => x.ShowHideGameOngoing, x => x.StartPauseMatchActionCommand.Visibility);
            this.BindCommand(ViewModel, x => x.SetPlayerOneAsCurrentServerCommand, x => x.PlayerOneIsServing);
            this.BindCommand(ViewModel, x => x.SetPlayerTwoAsCurrentServerCommand, x => x.PlayerTwoIsServing);
            this.BindCommand(ViewModel, x => x.PlayerOneFirstServeInCommand, x => x.PlayerOneFirstServe);
            this.BindCommand(ViewModel, x => x.PlayerOneFirstServeOutCommand, x => x.PlayerOneFirstServeOut);
            this.BindCommand(ViewModel, x => x.PlayerOneSecondServeInCommand, x => x.PlayerOneSecondServe);

            this.BindCommand(ViewModel, x => x.PlayerTwoFirstServeInCommand, x => x.PlayerTwoFirstServe);
            this.BindCommand(ViewModel, x => x.PlayerTwoFirstServeOutCommand, x => x.PlayerTwoFirstServeOut);
            this.BindCommand(ViewModel, x => x.PlayerTwoSecondServeInCommand, x => x.PlayerTwoSecondServe);
            this.BindCommand(ViewModel, x => x.UndoLastActionCommand);
            this.BindCommand(ViewModel, x => x.StartPauseMatchActionCommand);
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
