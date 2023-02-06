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

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<StocksPage>();
        builder.Services.AddTransient<StocksViewModel>();
        builder.Services.AddTransient<ChartPage>();
        builder.Services.AddTransient<ChartViewModel>();

        builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();
        builder.Services.AddSingleton<INotifier, PopupNotifier>();
        builder.Services.AddSingleton<IStockMarketClient, StockMarketClient>();
        builder.Services.AddSingleton<IStockMarketService, StockMarketService>();

        return builder.Build();
	}
}
