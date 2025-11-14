
namespace IphoneCollector.MVVM.View.CollectorViews;

public partial class CastPageView : ContentView
{
	public CastPageView()
	{
		InitializeComponent();

        this.Loaded += CastPageViewLoaded;
    }

    private void CastPageViewLoaded(object? sender, EventArgs e)
    {
        Radiobtn3.IsChecked = true;
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var radio = sender as RadioButton;

        if (radio.IsChecked) // only act when checked
        {
            if (radio.Content.ToString() == "iPhone Collections")
            {
                Grid1.IsVisible = true;
                Grid2.IsVisible = false;
            }
            else if (radio.Content.ToString() == "Status Details")
            {
                Grid1.IsVisible = false;
                Grid2.IsVisible = true;
            }
        }
    }
}