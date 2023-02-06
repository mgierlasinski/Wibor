using Wibor.ViewModels;

namespace Wibor;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.LoadData();
    }
}

