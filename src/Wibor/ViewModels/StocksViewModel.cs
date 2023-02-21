using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;
using Wibor.Models;
using Wibor.Services;

namespace Wibor.ViewModels;

public enum StocksVisualState
{
    SubPage, SideBySide
}

[ObservableObject]
public partial class StocksViewModel
{
    private const int LoadBatch = 3;

    private readonly IStockMarketService _stockMarketService;
    private readonly IDatabaseProvider _databaseProvider;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    public ChartViewModel Chart { get; set; }

    [ObservableProperty]
    private List<StockItem> _stockList;

    [ObservableProperty]
    private StockItem _selectedStock;

    [ObservableProperty]
    private bool _isProgressActive;

    [ObservableProperty]
    private double _progressValue;

    public StocksVisualState VisualState { get; set; }

    public StocksViewModel(
        IStockMarketService stockMarketService, 
        IDatabaseProvider databaseProvider,
        IDialogService dialogService,
        INavigationService navigationService,
        ChartViewModel chart)
    {
        _stockMarketService = stockMarketService;
        _databaseProvider = databaseProvider;
        _dialogService = dialogService;
        _navigationService = navigationService;
        Chart = chart;
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
                    DisplayName = stock.Name,
                    IsLoading = true
                };

                list.Add(item);
            }

            StockList = list;

            UpdateProgress();
            await FetchAllData();
            SelectDefaultItem();
        }
        catch (Exception e)
        {
            await _dialogService.ShowException(e);
        }
    }

    private async Task FetchAllData()
    {
        using var db = _databaseProvider.CreateDatabase();

        //foreach (var stockItem in StockList)
        //{
        //    await FetchData(db, stockItem);
        //}

        var uploadQueue = new List<StockItem>(StockList);
        while (uploadQueue.Any())
        {
            var tasks = uploadQueue.Take(LoadBatch).Select(x => FetchItemData(db, x));
            await Task.WhenAll(tasks);
            uploadQueue.RemoveRange(0, Math.Min(uploadQueue.Count, LoadBatch));
        }
    }

    private async Task FetchItemData(LiteDatabase db, StockItem item)
    {
        var week = await _stockMarketService.TakeLatest(db, item.StockId, DateTime.Now.AddDays(-7), DateTime.Now, 2);
        if (week.Count == 0)
            return;

        var latest = week[^1];
        item.Date = latest.Date;
        item.ValueCurrent = latest.Value;

        if (week.Count > 1)
        {
            item.CalculateChange(week[^2].Value);
        }

        item.IsLoading = false;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        ProgressValue = StockList.Count(x => !x.IsLoading) / (double)StockList.Count;
        IsProgressActive = ProgressValue < 1;
    }

    public void SelectDefaultItem()
    {
        if (IsProgressActive)
            return;

        SelectedStock = VisualState == StocksVisualState.SideBySide 
            ? StockList?.FirstOrDefault() 
            : null;
    }

    partial void OnSelectedStockChanged(StockItem value)
    {
        if (VisualState == StocksVisualState.SideBySide)
        {
            Chart.SelectedStock = value;
        }
        else if (value != null)
        {
            _navigationService.Navigate(AppRouting.ChartRoute, value);
        }
    }
}