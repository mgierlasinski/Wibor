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

    public MainViewModel(IStockMarketService stockMarketService)
    {
        _stockMarketService = stockMarketService;
    }

    public async Task LoadData()
    {
        Data = await _stockMarketService.GetData();
    }
}