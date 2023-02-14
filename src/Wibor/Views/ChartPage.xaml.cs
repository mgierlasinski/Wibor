using Wibor.ViewModels;

namespace Wibor.Views;

public partial class ChartPage : ContentPage
{
    private readonly ChartViewModel _viewModel;

    public ChartPage(ChartViewModel viewModel)
    {
        InitializeComponent();

        //_viewModel = viewModel;
        BindingContext = viewModel;
    }

    //protected override void OnNavigatedTo(NavigatedToEventArgs args)
    //{
    //    base.OnNavigatedTo(args);
    //    _viewModel.LoadData();
    //}
}