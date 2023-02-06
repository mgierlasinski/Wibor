using System.Collections.ObjectModel;
using AlohaKit.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Wibor.Models;
using Wibor.Services;

namespace Wibor.ViewModels;

[ObservableObject]
public partial class MainViewModel
{
    private readonly IStockMarketService _stockMarketService;

    [ObservableProperty]
    private List<StockEntity> _data;

    [ObservableProperty]
    private List<StockEntity> _lineData;

    [ObservableProperty]
    private ObservableCollection<ChartItem> _chartData;

    [ObservableProperty]
    private Stock _selectedStock = StockRepo.Wibor3M;

    public MainViewModel(IStockMarketService stockMarketService)
    {
        _stockMarketService = stockMarketService;
    }

    public async Task LoadData()
    {
        //_stockMarketService.ClearCache();

        Data = await _stockMarketService.FindAllBetween(_selectedStock.Id, DateTime.Now.AddMonths(-2), DateTime.Now);
        LineData = Data.OrderBy(x => x.Date).ToList();
        ChartData = new ObservableCollection<ChartItem>(Data.OrderBy(x => x.Date).Select(x => new ChartItem
        {
            Label = x.DateLabel,
            Value = (float)x.Value
        }));
    }

    partial void OnSelectedStockChanged(Stock value)
    {
        LoadData();
    }
}