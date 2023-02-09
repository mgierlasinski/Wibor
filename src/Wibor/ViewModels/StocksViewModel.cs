using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wibor.Models;
using Wibor.Services;

namespace Wibor.ViewModels;

[ObservableObject]
public partial class StocksViewModel
{
    private readonly IStockMarketService _stockMarketService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private List<StockItem> _stockList;

    [ObservableProperty]
    private StockItem _selectedStock;

    public StocksViewModel(
        IStockMarketService stockMarketService, 
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _stockMarketService = stockMarketService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task LoadData()
    {
        try
        {
            var list = new List<StockItem>();

            foreach (var stock in _stockMarketService.GetStocks())
            {
                var item = new StockItem
                {
                    StockId = stock.Id,
                    DisplayName = stock.Name
                };

                var week = await _stockMarketService.TakeLatest(stock.Id, DateTime.Now.AddDays(-7), DateTime.Now, 2);
                if (week.Count == 0)
                    continue;

                var latest = week[^1];
                item.Date = latest.Date;
                item.ValueCurrent = latest.Value;

                if (week.Count > 1)
                {
                    item.CalculateChange(week[^2].Value);
                }

                list.Add(item);
            }

            StockList = list;
        }
        catch (Exception e)
        {
            await _dialogService.ShowException(e);
        }
    }

    partial void OnSelectedStockChanged(StockItem value)
    {
        _navigationService.Navigate(AppRouting.ChartRoute, value);
    }
}