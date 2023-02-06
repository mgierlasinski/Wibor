namespace Wibor.Models;

public class StockUpdate
{
    public long Id { get; set; }
    public string StockId { get; set; }
    public DateTime LastUpdate { get; set; }
    public DateTime RangeFrom { get; set; }
    public DateTime RangeTo { get; set; }
}