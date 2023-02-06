using Wibor.Models;

namespace Wibor.Services;

public interface IStockMarketService
{
    Task<List<StockEntity>> GetData();
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

    public async Task<List<StockEntity>> GetData()
    {
        using var db = _databaseProvider.CreateDatabase();

        //db.DropCollection("StockEntity");
        //db.DropCollection("StockUpdate");

        var updateCol = db.GetCollection<StockUpdate>();
        var dataCol = db.GetCollection<StockEntity>();
        
        var update = updateCol.FindOne(x => x.Id > 0);
        if (update == null)
        {
            var data = await _stockMarketClient.GetData(DateTime.Now.AddDays(-6), DateTime.Now);
            dataCol.InsertBulk(data.Select(Map));
            updateCol.Insert(new StockUpdate { LastUpdate = DateTime.Now });
        }
        else if (update.LastUpdate.Date < DateTime.Now.Date)
        {
            var data = await _stockMarketClient.GetData(update.LastUpdate.AddDays(1), DateTime.Now);
            dataCol.InsertBulk(data.Select(Map));
            update.LastUpdate = DateTime.Now;
            updateCol.Update(update);
        }

        return dataCol.FindAll().OrderByDescending(x => x.Date).ToList();
    }
    
    private StockEntity Map(StockData data)
    {
        return new StockEntity
        {
            Date = data.Date,
            Value = data.Open
        };
    }
}