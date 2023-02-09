namespace Wibor.Models;

public record Stock(string Id, string Name);

public record StockData(string StockId, DateTime Date, double Open, double Highest, double Lowest, double Close);