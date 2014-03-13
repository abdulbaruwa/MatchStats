using ReactiveUI;
using Windows.UI.Xaml;
using MatchStats.ViewModels;

namespace MatchStats.Controls
{
    public sealed partial class ProfileUserControl : IViewFor<UserProfileViewModel>
    {
        public ProfileUserControl()
        {
            this.InitializeComponent();
            this.Bind(ViewModel, x => x.Ratings, x => x.PlayerRating.ItemsSource);
            this.Bind(ViewModel, x => x.PlayerFirstName, x => x.PlayerFirstName);
            this.Bind(ViewModel, x => x.PlayerSurname, x => x.PlayerLastName);
            this.BindCommand(ViewModel, x => x.NavAwayCommand);
            this.BindCommand(ViewModel, x => x.UpdateProfileCommand);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (UserProfileViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(UserProfileViewModel), typeof(ProfileUserControl), new PropertyMetadata(null));

        public UserProfileViewModel ViewModel
        {
            get { return (UserProfileViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
