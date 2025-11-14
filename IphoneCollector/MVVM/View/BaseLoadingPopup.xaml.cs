using CommunityToolkit.Maui.Views;
using IphoneCollector.MVVM.ViewModel;

namespace IphoneCollector.MVVM.View;

public partial class BaseLoadingPopup : Popup
{
	public BaseLoadingPopup(MainViewModelVM viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;

    }
}