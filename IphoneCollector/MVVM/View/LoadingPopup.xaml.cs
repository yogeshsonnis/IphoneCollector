using CommunityToolkit.Maui.Views;
using IphoneCollector.MVVM.ViewModel;

namespace IphoneCollector.MVVM.View;

public partial class LoadingPopup : Popup
{
	public LoadingPopup(MainViewModel viewModel)
	{
		InitializeComponent();
        // Force the popup to take full screen
        this.Size = new Size(DeviceDisplay.MainDisplayInfo.Width, DeviceDisplay.MainDisplayInfo.Height);
        BindingContext = viewModel; 

    }
}