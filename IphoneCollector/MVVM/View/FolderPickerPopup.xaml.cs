using CommunityToolkit.Maui.Views;

namespace IphoneCollector.MVVM.View;

public partial class FolderPickerPopup : Popup
{
    public FolderPickerPopup(object bindingContext)
    {
        InitializeComponent();
        BindingContext = bindingContext;
    }


}