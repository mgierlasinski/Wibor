using Wibor.Services;
using Wibor.ViewModels;

namespace Wibor;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();
        builder.Services.AddSingleton<IStockMarketClient, StockMarketClient>();
        builder.Services.AddSingleton<IStockMarketService, StockMarketService>();

        return builder.Build();
	}
}
