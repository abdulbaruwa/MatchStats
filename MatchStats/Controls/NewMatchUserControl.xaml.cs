using MatchStats.ViewModels;
using Windows.UI.Xaml;
using ReactiveUI;

namespace MatchStats.Controls
{
    public sealed partial class NewMatchUserControl : IViewFor<NewMatchControlViewModel>
    {
        public NewMatchUserControl()
        {
            this.InitializeComponent();
            
            this.BindCommand(ViewModel, x => x.SaveCommand);

            this.Bind(ViewModel, x => x.UseDefaultPlayer, x => x.UseDefaultPlayer.IsChecked);

            this.Bind(ViewModel, x => x.Grades, x => x.Grade.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedGrade, x => x.Grade.SelectedValue);

            this.Bind(ViewModel, x => x.FinalSet, x => x.FinalSetFormat.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedFinalSet, x => x.FinalSetFormat.SelectedValue);

            this.Bind(ViewModel, x => x.SetsFormats, x => x.Sets.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedSetsFormat, x => x.Sets.SelectedValue);

            this.Bind(ViewModel, x => x.AgeGroups, x => x.AgeGroup.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedAgeGroup, x => x.AgeGroup.SelectedValue);

            this.Bind(ViewModel, x => x.DueceFormats, x => x.DueceFormat.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedDueceFormat, x => x.DueceFormat.SelectedValue);

            this.Bind(ViewModel, x => x.TournamentName, x => x.Tournament.Text);

            this.Bind(ViewModel, x => x.PlayerTwoFirstName, x => x.PlayerTwoFirstName.Text);
            this.Bind(ViewModel, x => x.PlayerTwoLastName, x => x.PlayerTwoLastName.Text);

            this.Bind(ViewModel, x => x.PlayerOneFirstName, x => x.PlayerOneFirstName.Text);
            this.Bind(ViewModel, x => x.PlayerOneLastName, x => x.PlayerOneLastName.Text);

            this.Bind(ViewModel, x => x.Ratings, x => x.PlayerOneRating.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedPlayerOneRating, x => x.PlayerOneRating.SelectedValue);

            this.Bind(ViewModel, x => x.Ratings, x => x.PlayerTwoRating.ItemsSource);
            this.Bind(ViewModel, x => x.SelectedPlayerTwoRating, x => x.PlayerTwoRating.SelectedValue);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                                                                                                typeof(NewMatchControlViewModel),
                                                                                                typeof(NewMatchUserControl),
                                                                                                new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NewMatchControlViewModel)value; }
        }

        public NewMatchControlViewModel ViewModel
        {
            get { return (NewMatchControlViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
