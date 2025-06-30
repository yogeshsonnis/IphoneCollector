using CommunityToolkit.Maui.Views;
using IphoneCollector.MVVM.ViewModel;

namespace IphoneCollector.MVVM.View;

public partial class LoadingPopup : Popup
{
    public LoadingPopup(MainViewModel viewModel)
    {
        InitializeComponent();
        // Force the popup to take full screen
        //this.Size = new Size(DeviceDisplay.MainDisplayInfo.Width, DeviceDisplay.MainDisplayInfo.Height);
        //BindingContext = viewModel; 

        //var displayInfo = DeviceDisplay.MainDisplayInfo;

        //// Convert physical pixels to DIPs (device-independent units)
        //double width = displayInfo.Width / displayInfo.Density;
        //double height = displayInfo.Height / displayInfo.Density;

        //this.Size = new Size(width, height);
        BindingContext = viewModel;


    }
}