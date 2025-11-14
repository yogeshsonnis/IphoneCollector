using IphoneCollector.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IphoneCollector.MVVM.View.CollectorViews;

public partial class CollectionWizardView : ContentView
{
   
    public CollectionWizardView()
    {
        InitializeComponent();

       
    }
   
    

   
    private void RadioButton_Changed(object sender, CheckedChangedEventArgs e)
    {
        var radio = sender as RadioButton;

        if (radio.IsChecked) // only act when checked
        {
            if (radio.Content.ToString() == "Add Device")
            {
                Grid3.IsVisible = true;
                Grid4.IsVisible = false;
            }
            else if (radio.Content.ToString() == "Phone List")
            {
                Grid3.IsVisible = false;
                Grid4.IsVisible = true;
            }
        }
    }

    private async void OnSaveAddClicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.DisplayAlert("This Page Says ", "Custodian and iphone model are required", "OK");
    }
}