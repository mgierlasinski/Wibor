namespace Wibor.Models;

public class StockDaily
{
    public long Id { get; set; }
    public string StockId { get; set; }
    public DateTime Date { get; set; }
    public double Value { get; set; }

    public static StockDaily FromData(StockData data) => new StockDaily
    {
        StockId = data.StockId,
        Date = data.Date,
        Value = data.Open
    };
}