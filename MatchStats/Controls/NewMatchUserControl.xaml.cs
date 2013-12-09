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
            this.Bind(ViewModel, x => x.PlayerTwoFirstName, x => x.PlayerTwoFirstName.Text);
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
