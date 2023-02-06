using LiteDB;
using Wibor.Models;

namespace Wibor.Services;

public interface IStockMarketService
{
    Stock[] GetStocks();
    Task<List<StockEntity>> FindAllBetween(string stockId, DateTime dateFrom, DateTime dateTo);
    Task<List<StockEntity>> TakeLatest(string stockId, DateTime dateFrom, DateTime dateTo, int takeAmount);
    void ClearCache();
}

public class StockMarketService : IStockMarketService
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly IStockMarketClient _stockMarketClient;

    public StockMarketService(
        IDatabaseProvider databaseProvider,
        IStockMarketClient stockMarketClient)
    {
        _databaseProvider = databaseProvider;
        _stockMarketClient = stockMarketClient;
    }

    public Stock[] GetStocks() => new[]
    {
        StockRepo.Wibor1M,
        StockRepo.Wibor3M,
        StockRepo.Wibor6M,
        StockRepo.Wibor1Y
    };

    public async Task<List<StockEntity>> FindAllBetween(string stockId, DateTime dateFrom, DateTime dateTo)
    {
        dateFrom = dateFrom.Date;
        dateTo = dateTo.Date;

        using var db = _databaseProvider.CreateDatabase();
        await LoadCache(db, stockId, dateFrom, dateTo);
        
        return db.GetCollection<StockEntity>()
            .Find(x => x.StockId == stockId && x.Date >= dateFrom && x.Date <= dateTo)
            .OrderBy(x => x.Date)
            .ToList();
    }

    public async Task<List<StockEntity>> TakeLatest(string stockId, DateTime dateFrom, DateTime dateTo, int takeAmount)
    {
        dateFrom = dateFrom.Date;
        dateTo = dateTo.Date;

        using var db = _databaseProvider.CreateDatabase();
        await LoadCache(db, stockId, dateFrom, dateTo);

        return db.GetCollection<StockEntity>()
            .Find(x => x.StockId == stockId && x.Date >= dateFrom && x.Date <= dateTo)
            .OrderBy(x => x.Date)
            .TakeLast(takeAmount)
            .ToList();
    }

    private async Task LoadCache(LiteDatabase db, string stockId, DateTime dateFrom, DateTime dateTo)
    {
        var updateCol = db.GetCollection<StockUpdate>();
        var update = updateCol.FindOne(x => x.StockId == stockId);

        if (update == null)
        {
            var data = await _stockMarketClient.GetData(stockId, dateFrom, dateTo);
            db.GetCollection<StockEntity>().InsertBulk(data.Select(StockEntity.Map));
            updateCol.Insert(new StockUpdate
            {
                StockId = stockId,
                LastUpdate = DateTime.Now,
                RangeFrom = dateFrom,
                RangeTo = dateTo
            });
        }
        else
        {
            var isUpdateDirty = false;
            if (dateFrom < update.RangeFrom)
            {
                var data = await _stockMarketClient.GetData(stockId, dateFrom, update.RangeFrom.AddDays(-1));
                db.GetCollection<StockEntity>().InsertBulk(data.Select(StockEntity.Map));
                update.RangeFrom = dateFrom;
                isUpdateDirty = true;
            }
            if (dateTo > update.RangeTo)
            {
                var data = await _stockMarketClient.GetData(stockId, update.RangeTo.AddDays(1), dateTo);
                db.GetCollection<StockEntity>().InsertBulk(data.Select(StockEntity.Map));
                update.RangeTo = dateTo;
                isUpdateDirty = true;
            }

            if (isUpdateDirty)
            {
                updateCol.Update(update);
            }
        }
    }

    public void ClearCache()
    {
        using var db = _databaseProvider.CreateDatabase();

        db.DropCollection(nameof(StockEntity));
        db.DropCollection(nameof(StockUpdate));
    }
}