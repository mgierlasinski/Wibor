namespace Wibor.Models;

public record Stock(string Id, string Name);

public static class StockRepo
{
    public static readonly Stock Wibor1M = new("PLOPLN1M", "Wibor 1M");
    public static readonly Stock Wibor3M = new("PLOPLN3M", "Wibor 3M");
    public static readonly Stock Wibor6M = new("PLOPLN6M", "Wibor 6M");
    public static readonly Stock Wibor1Y = new("PLOPLN1Y", "Wibor 1Y");

    public static Stock[] All { get; } = 
    {
        Wibor1M,
        Wibor3M,
        Wibor6M,
        Wibor1Y
    };
}