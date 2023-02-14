using LiteDB;
using Wibor.Models;

namespace Wibor.Services;

public interface ICacheService
{
    Task LoadCache(LiteDatabase db, string stockId, DateTime dateFrom, DateTime dateTo);
    void ClearCache();
    CacheInfo GetCacheInfo();
}

public class CacheService : ICacheService
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly IStockMarketClient _stockMarketClient;

    public CacheService(
        IDatabaseProvider databaseProvider, 
        IStockMarketClient stockMarketClient)
    {
        _databaseProvider = databaseProvider;
        _stockMarketClient = stockMarketClient;
    }

    public async Task LoadCache(LiteDatabase db, string stockId, DateTime dateFrom, DateTime dateTo)
    {
        var updateCol = db.GetCollection<StockUpdate>();
        var update = updateCol.FindOne(x => x.StockId == stockId);

        if (update == null)
        {
            var data = await _stockMarketClient.GetData(stockId, dateFrom, dateTo);
            db.GetCollection<StockDaily>().InsertBulk(data.Select(StockDaily.FromData));
            updateCol.Insert(new StockUpdate
            {
                StockId = stockId,
                LastUpdate = DateTime.Now,
                RangeFrom = dateFrom,
                RangeTo = dateTo
            });
        }
        else if (update.LastUpdate < DateTime.Now.Date || dateFrom < update.RangeFrom || dateTo > update.RangeTo)
        {
            var query = db.GetCollection<StockDaily>().Find(x => x.StockId == stockId);
            var oldest = query.MinBy(x => x.Date);
            var newest = query.MaxBy(x => x.Date);

            if (dateFrom < oldest?.Date)
            {
                var data = await _stockMarketClient.GetData(stockId, dateFrom, oldest.Date.AddDays(-1));
                db.GetCollection<StockDaily>().InsertBulk(data.Select(StockDaily.FromData));
            }

            if (dateTo > newest?.Date)
            {
                var data = await _stockMarketClient.GetData(stockId, newest.Date.AddDays(1), dateTo);
                db.GetCollection<StockDaily>().InsertBulk(data.Select(StockDaily.FromData));
            }

            update.LastUpdate = DateTime.Now;
            update.RangeFrom = dateFrom;
            update.RangeTo = dateTo;
            updateCol.Update(update);
        }
    }

    public void ClearCache()
    {
        using var db = _databaseProvider.CreateDatabase();

        db.DropCollection(nameof(StockDaily));
        db.DropCollection(nameof(StockUpdate));
    }

    public CacheInfo GetCacheInfo()
    {
        using var db = _databaseProvider.CreateDatabase();

        var lastUpdated = db.GetCollection<StockUpdate>().FindAll().MaxBy(x => x.LastUpdate)?.LastUpdate;
        var totalRows = db.GetCollection<StockDaily>().Count(Query.All());

        return new CacheInfo(lastUpdated, totalRows);
    }
}