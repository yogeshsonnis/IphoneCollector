namespace IphoneCollector.MVVM.View.CollectorViews;

public partial class ImagingWizardView : ContentView
{
	public ImagingWizardView()
	{
		InitializeComponent();
	}

    private async void OnStartImagingClicked(object sender, EventArgs e)
    {
        
        ImagingProgressBar.Progress = 0;

        
        for (double i = 0; i <= 1; i += 0.07)          // Simulate imaging process (e.g., loading)
        {
            ImagingProgressBar.Progress = i;
            await Task.Delay(800);                     // simulate work for each step
        }

        // Optional: show message when done

        await Application.Current.MainPage.DisplayAlert("Done", "Imaging completed successfully!", "OK");
    }
}