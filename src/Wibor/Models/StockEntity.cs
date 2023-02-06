namespace Wibor.Models;

public class StockEntity
{
    public long Id { get; set; }
    public string StockId { get; set; }
    public DateTime Date { get; set; }
    public double Value { get; set; }

    public string DateLabel => $"{Date:dd MMM}";

    public static StockEntity Map(StockData data) => new StockEntity
    {
        StockId = data.StockId,
        Date = data.Date,
        Value = data.Open
    };
}