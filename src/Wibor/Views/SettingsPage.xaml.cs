using Wibor.ViewModels;

namespace Wibor.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}