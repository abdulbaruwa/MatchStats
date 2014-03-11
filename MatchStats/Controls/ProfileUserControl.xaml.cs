using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using MatchStats.ViewModels;
using ReactiveUI;

namespace MatchStats.Controls
{
    public sealed partial class ProfileUserControl : IViewFor<UserProfileViewModel>
    {
        public ProfileUserControl()
        {
            this.InitializeComponent();
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
