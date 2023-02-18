using LiteDB;
using Wibor.Models;

namespace Wibor.Services;

public interface IStockMarketService
{
    Stock[] GetStocks();
    Task<List<StockDaily>> FindAllBetween(string stockId, DateTime dateFrom, DateTime dateTo);
    Task<List<StockDaily>> TakeLatest(string stockId, DateTime dateFrom, DateTime dateTo, int takeAmount);
}

public class StockMarketService : IStockMarketService
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ICacheService _cacheService;

    public StockMarketService(
        IDatabaseProvider databaseProvider,
        ICacheService cacheService)
    {
        _databaseProvider = databaseProvider;
        _cacheService = cacheService;
    }

    public Stock[] GetStocks() => new[]
    {
        new Stock("PLOPLN1M", "Wibor 1M"),
        new Stock("PLOPLN3M", "Wibor 3M"),
        new Stock("PLOPLN6M", "Wibor 6M"),
        new Stock("PLOPLN1Y", "Wibor 1Y"),
        new Stock("PLOPLNON", "Wibor Overnight"),
        new Stock("PLOPLNTN", "Wibor TN"),
        new Stock("PLOPLN1W", "Wibor 1W"),
        new Stock("PLOPLN2W", "Wibor 2W")
    };

    public async Task<List<StockDaily>> FindAllBetween(string stockId, DateTime dateFrom, DateTime dateTo)
    {
        dateFrom = dateFrom.Date;
        dateTo = dateTo.Date;

        using var db = _databaseProvider.CreateDatabase();
        await _cacheService.LoadCache(db, stockId, dateFrom, dateTo);
        
        return db.GetCollection<StockDaily>()
            .Find(x => x.StockId == stockId && x.Date >= dateFrom && x.Date <= dateTo)
            .OrderBy(x => x.Date)
            .ToList();
    }

    public async Task<List<StockDaily>> TakeLatest(string stockId, DateTime dateFrom, DateTime dateTo, int takeAmount)
    {
        dateFrom = dateFrom.Date;
        dateTo = dateTo.Date;

        using var db = _databaseProvider.CreateDatabase();
        await _cacheService.LoadCache(db, stockId, dateFrom, dateTo);

        return db.GetCollection<StockDaily>()
            .Find(x => x.StockId == stockId && x.Date >= dateFrom && x.Date <= dateTo)
            .OrderBy(x => x.Date)
            .TakeLast(takeAmount)
            .ToList();
    }
}