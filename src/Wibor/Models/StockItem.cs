﻿namespace Wibor.Models;

public class StockItem
{
    public string StockId { get; set; }
    public string DisplayName { get; set; }
    public DateTime Date { get; set; }
    public double ValueCurrent { get; set; }
    public double ValueChange { get; set; }

    public string ValueChangeDisplay => ValueChange switch
    {
        > 0 => $"+{ValueChange}% 🔺",
        < 0 => $"{ValueChange}% 🔻",
        _ => "0% 🔴"
    };

    public Color ValueChangeColor => ValueChange switch
    {
        < 0 => Colors.Red,
        > 0 => Colors.Green,
        _ => Colors.Gray
    };

    public void CalculateChange(double prevValue)
    {
        ValueChange = Math.Round(ValueCurrent - prevValue, 2);
    }
}