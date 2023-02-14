using Wibor.ViewModels;

namespace Wibor.Views;

public partial class StocksPage : ContentPage
{
    private readonly StocksViewModel _viewModel;
    private ChartView _chartView;
    private double _lastWidth;

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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (_lastWidth == width)
            return;

        _lastWidth = width;
        SwitchVisualState(width > 600 ? StocksVisualState.SideBySide : StocksVisualState.SubPage);
    }

    private void SwitchVisualState(StocksVisualState visualState)
    {
        if (visualState == _viewModel.VisualState)
            return;

        _viewModel.VisualState = visualState;
        _viewModel.SelectDefaultItem();

        if (visualState == StocksVisualState.SubPage)
        {
            if (_chartView != null)
            {
                MainGrid.Remove(_chartView);
            }
            MainGrid.ColumnDefinitions.Clear();

            // https://github.com/dotnet/maui/pull/13137
            //StocksCollection.ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical);
        }
        else
        {
            _chartView ??= new ChartView { BindingContext = _viewModel.Chart };

            MainGrid.ColumnDefinitions.Clear();
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(250)));
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            MainGrid.Add(_chartView, 1);

            // https://github.com/dotnet/maui/pull/13137
            //StocksCollection.ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical);
        }
    }
}