namespace IphoneCollector.MVVM.View.CollectorViews;

public partial class StorageWizardView : ContentView
{
	public StorageWizardView()
	{
		InitializeComponent();
	}
    private void RadioButton_StgWizardChanged(object sender, CheckedChangedEventArgs e)
    {
        var radio = sender as RadioButton;

        if (radio.IsChecked) // only act when checked
        {
            if (radio.Content.ToString() == "Storage Address Book")
            {
                Grid5.IsVisible = true;
                Grid6.IsVisible = false;
            }
            else if (radio.Content.ToString() == "Storage Setup")
            {
                Grid5.IsVisible = false;
                Grid6.IsVisible = true;
            }
        }
    }
}