using IphoneCollector.MVVM.ViewModel;

namespace IphoneCollector
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        private void OnSettingsTapped(object sender, TappedEventArgs e)
        {
            SettingsPicker.IsVisible = !SettingsPicker.IsVisible;
            SettingsPicker.Focus(); // triggers dropdown
        }
    }

}
