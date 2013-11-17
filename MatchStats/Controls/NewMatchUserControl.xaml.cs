using MatchStats.ViewModels;
using Windows.UI.Xaml;
using ReactiveUI;

namespace MatchStats.Controls
{
    public sealed partial class NewMatchUserControl : IViewFor<NewMatchControlViewModel>
    {
        public NewMatchUserControl()
        {
            ViewModel = new NewMatchControlViewModel();
            this.InitializeComponent();
            this.BindCommand(ViewModel, x => x.SaveCommand);
            this.DataContext = ViewModel;
        }

        public static readonly DependencyProperty ControlViewModelProperty = DependencyProperty.Register("ControlControlViewModel",
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
            get { return (NewMatchControlViewModel) GetValue(ControlViewModelProperty); }
            set { SetValue(ControlViewModelProperty, value); }
        }
    }
}
