using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wibor.Services;

namespace Wibor.ViewModels;

[ObservableObject]
public partial class SettingsViewModel
{
    private readonly ICacheService _cacheService;

    [ObservableProperty]
    private string _lastUpdated;

    [ObservableProperty]
    private int _totalRows;

    public SettingsViewModel(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void LoadInfo()
    {
        var info = _cacheService.GetCacheInfo();
        LastUpdated = info.LastUpdated?.ToString("ddd, dd MMMM, hh:mm") ?? "Never updated";
        TotalRows = info.TotalRows;
    }

    [RelayCommand]
    private void ClearCache()
    {
        _cacheService.ClearCache();
        LoadInfo();
    }
}