using IphoneCollector.MVVM.ViewModel;

namespace IphoneCollector
{
    public partial class MainPage : ContentPage
    {
        MainViewModelVM vm;
        public MainPage(MainViewModelVM viewModel)
        {
            InitializeComponent();
            vm = viewModel;
            BindingContext = viewModel;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object? sender, EventArgs e)
        {
            await vm.GetCollectorWizardData();
        }

        private void OnSettingsTapped(object sender, TappedEventArgs e)
        {
            SettingsPicker.IsVisible = !SettingsPicker.IsVisible;
            SettingsPicker.Focus(); // triggers dropdown
        }
    }

}
