using System;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using ReactiveUI;
using Windows.UI.Xaml;
using MatchStats.ViewModels;
using Windows.UI.Xaml.Controls;

namespace MatchStats.Views
{
    public sealed partial class MatchesPlayedView : IViewFor<MatchesPlayedViewModel>
    {
        private Rect _windowBounds;

        public MatchesPlayedView()
        {
            this.InitializeComponent();
            this.BindCommand(ViewModel, x => x.AddMatch);
            this.BindCommand(ViewModel, x => x.EditProfile);
            this.Bind(ViewModel, x => x.MyMatchStats, x => x.itemGridView.ItemsSource);
            this.Bind(ViewModel, x => x.ShowNewMatchPopup, x => x.AddMatchDialog.IsOpen);
            this.Bind(ViewModel, x => x.UserProfileViewModel.ShowMe, x => x.EditProfilePoupup.IsOpen);
            this.Bind(ViewModel, x => x.UserProfileViewModel, x => x.EditProfileControl.ViewModel);

            itemGridView.Events().ItemClick.Subscribe(x =>
            {
                //Having to first set to null otherwise RxUI's  WhenAny never fires if the item passed is the same as the last
                ViewModel.SelectedMatchStat = null;
                ViewModel.SelectedMatchStat = x.ClickedItem;
            });

            _windowBounds = Window.Current.Bounds;
            // Added to listen for events when the window size is updated.
            EditProfilePoupup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (_windowBounds.Width - 646) : 0);
            EditProfilePoupup.SetValue(Canvas.TopProperty, 0);
            Window.Current.SizeChanged += OnWindowSizeChanged;
            EditProfileControl.Height = _windowBounds.Height;
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            _windowBounds = Window.Current.Bounds;
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
