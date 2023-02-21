using CommunityToolkit.Mvvm.ComponentModel;
using Wibor.Models;
using Wibor.Services;

namespace Wibor.ViewModels;

[ObservableObject]
[QueryProperty(nameof(SelectedStock), nameof(StockItem))]
public partial class ChartViewModel
{
    private readonly IStockMarketService _stockMarketService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private List<StockDaily> _data;

    [ObservableProperty]
    private double _minValue;

    [ObservableProperty]
    private double _maxValue;
    
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private LinearGradientBrush _fillBrush;

    [ObservableProperty]
    private StockItem _selectedStock;

    [ObservableProperty]
    private ChartRange _selectedRange;

    public ChartRange[] Ranges { get; } = 
    {
        new ChartRange("1M", DateTime.Now.AddMonths(-1), DateTime.Now),
        new ChartRange("3M", DateTime.Now.AddMonths(-2), DateTime.Now),
        new ChartRange("6M", DateTime.Now.AddMonths(-6), DateTime.Now),
        new ChartRange("1Y", DateTime.Now.AddYears(-1), DateTime.Now)
    };

    public ChartViewModel(IStockMarketService stockMarketService, IDialogService dialogService)
    {
        _stockMarketService = stockMarketService;
        _dialogService = dialogService;
        _selectedRange = Ranges[0];
    }

    public async Task LoadData()
    {
        if (SelectedStock == null)
            return;

        try
        {
            IsLoading = true;
            Data = await _stockMarketService.FindAllBetween(SelectedStock.StockId, SelectedRange.From, SelectedRange.To);
            MinValue = Data.Min(x => x.Value);
            MaxValue = Data.Max(x => x.Value);

            SetupFill();
        }
        catch (Exception e)
        {
            await _dialogService.ShowException(e);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SetupFill()
    {
        var baseColor = Application.Current.GetResource<Color>("Primary");
        var color1 = baseColor.MakeTransparent(0.8f);
        var color2 = baseColor.MakeTransparent(0.2f);
        var gradientEnd = 1 - (float)(MinValue / MaxValue);
        
        FillBrush = new LinearGradientBrush(new GradientStopCollection
        {
            new(color1, 0.0f),
            new(color2, gradientEnd)
        },
        startPoint: new Point(0.5, 0),
        endPoint: new Point(0.5, 1));
    }

    partial void OnSelectedStockChanged(StockItem value)
    {
        LoadData();
    }

    partial void OnSelectedRangeChanged(ChartRange value)
    {
        LoadData();
    }
}