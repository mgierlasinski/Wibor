using Syncfusion.Maui.Core.Hosting;
using Wibor.Services;
using Wibor.ViewModels;
using Wibor.Views;

namespace Wibor;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddTransient<StocksPage>();
        builder.Services.AddTransient<StocksViewModel>();
        builder.Services.AddTransient<ChartPage>();
        builder.Services.AddTransient<ChartViewModel>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();
        builder.Services.AddSingleton<ICacheService, CacheService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IStockMarketClient, StockMarketClient>();
        builder.Services.AddSingleton<IStockMarketService, StockMarketService>();

        return builder.Build();
	}
}
