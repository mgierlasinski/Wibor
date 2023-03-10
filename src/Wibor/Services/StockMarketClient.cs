using System.Globalization;
using Wibor.Models;

namespace Wibor.Services;

public interface IStockMarketClient
{
    Task<List<StockData>> GetData(string stockId, DateTime dateFrom, DateTime dateTo);
}

public class StockMarketClient : IStockMarketClient
{
    private readonly HttpClient _client = new();

    public async Task<List<StockData>> GetData(string stockId, DateTime dateFrom, DateTime dateTo)
    {
        var url = $"https://stooq.pl/q/d/l/?s={stockId}&d1={dateFrom:yyyyMMdd}&d2={dateTo:yyyyMMdd}&i=d";
        var response = await _client.GetAsync(url);

        var csv = await response.Content.ReadAsStringAsync();
        var lines = csv.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        
        return lines.Skip(1).Select(x => ParseLine(stockId, x)).ToList();
    }

    private StockData ParseLine(string stockId, string line)
    {
        double Value(string col) => double.Parse(col, CultureInfo.InvariantCulture);

        var cols = line.Split(',');
        return new StockData(
            StockId: stockId,
            Date: DateTime.ParseExact(cols[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Open: Value(cols[1]),
            Highest: Value(cols[2]),
            Lowest:Value(cols[3]), 
            Close: Value(cols[4]));
    }
}