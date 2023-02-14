using AlohaKit.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
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
    private ObservableCollection<ChartItem> _chartItems;

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
            Data = await _stockMarketService.FindAllBetween(SelectedStock.StockId, SelectedRange.From, SelectedRange.To);
            ChartItems = new ObservableCollection<ChartItem>(Data.Select(x => new ChartItem
            {
                Label = $"{x.Date:dd MMM}",
                Value = (float)x.Value
            }));
        }
        catch (Exception e)
        {
            await _dialogService.ShowException(e);
        }
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