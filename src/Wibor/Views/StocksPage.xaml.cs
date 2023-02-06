using Wibor.ViewModels;

namespace Wibor.Views;

public partial class StocksPage : ContentPage
{
    private readonly StocksViewModel _viewModel;

    public StocksPage(StocksViewModel viewModel)
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