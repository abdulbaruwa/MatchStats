using Windows.UI.Xaml;
using MatchStats.ViewModels;
using ReactiveUI;

namespace MatchStats.Views
{
    public sealed partial class MatchScoreView : IViewFor<MatchScoreViewModel>
    {
        public MatchScoreView()
        {
            //ControlViewModel = new MatchScoreViewModel();
            ViewModel =  new ViewModelLocator().MatchScoreViewModel;
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.PointReasons, x => x.PlayerOneCommands.ItemsSource);
            this.OneWayBind(ViewModel, x => x.PointReasons, x => x.PlayerTwoCommands.ItemsSource);
            this.Bind(ViewModel, x => x.PlayerOneCurrentGame, x => x.PlayerOneCurrentGame.Text);
            this.Bind(ViewModel, x => x.PlayerTwoCurrentGame, x => x.PlayerTwoCurrentGame.Text);
            this.Bind(ViewModel, x => x.PlayerOneFirstSet, x => x.PlayerOneFirstSet.Text);
            this.Bind(ViewModel, x => x.PlayerTwoFirstSet, x => x.PlayerTwoFirstSet.Text);
            this.Bind(ViewModel, x => x.PlayerOneSecondSet, x => x.PlayerOneSecondSet.Text);
            this.Bind(ViewModel, x => x.PlayerTwoSecondSet, x => x.PlayerTwoSecondSet.Text);
            this.Bind(ViewModel, x => x.PlayerOneThirdSet, x => x.PlayerOneThirdSet.Text);
            this.Bind(ViewModel, x => x.PlayerTwoThirdSet, x => x.PlayerTwoThirdSet.Text);
            this.Bind(ViewModel, x => x.PlayerOnesName, x => x.PlayerOnesName.Text);
            this.Bind(ViewModel, x => x.PlayerTwosName, x => x.PlayerTwosName.Text);
            this.BindCommand(ViewModel, x => x.NavToHomePageCommand, x => x.backButton);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ControlViewModel", typeof (MatchScoreViewModel), typeof (MatchScoreView), new PropertyMetadata(null));


        

        public MatchScoreViewModel ViewModel
        {
            get { return (MatchScoreViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        } 

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel =(MatchScoreViewModel) value; }
        }

    }
}
