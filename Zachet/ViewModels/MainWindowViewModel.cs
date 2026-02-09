using CommunityToolkit.Mvvm.ComponentModel;

namespace Zachet.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentViewModel = new MainViewModel();

        public static MainWindowViewModel Instace = new();

        public MainWindowViewModel()
        {
            Instace = this;
        }
    }
}
