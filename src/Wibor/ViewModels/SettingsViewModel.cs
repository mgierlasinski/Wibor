using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wibor.Services;

namespace Wibor.ViewModels;

[ObservableObject]
public partial class SettingsViewModel
{
    private readonly IStockMarketService _stockMarketService;

    public SettingsViewModel(IStockMarketService stockMarketService)
    {
        _stockMarketService = stockMarketService;
    }

    [RelayCommand]
    private void ClearCache()
    {
        _stockMarketService.ClearCache();
    }
}