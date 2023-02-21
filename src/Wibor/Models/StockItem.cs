using CommunityToolkit.Mvvm.ComponentModel;

namespace Wibor.Models;

[ObservableObject]
public partial class StockItem
{
    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private double _valueCurrent;

    [ObservableProperty]
    private double _valueChange;

    [ObservableProperty]
    private bool _isLoading;

    public string StockId { get; set; }
    public string DisplayName { get; set; }
    
    partial void OnValueChangeChanged(double value)
    {
        OnPropertyChanged(nameof(ValueChangeDisplay));
        OnPropertyChanged(nameof(ValueChangeColor));
    }

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