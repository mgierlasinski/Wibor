using Wibor.Views;

namespace Wibor;

public static class AppRouting
{
    public const string ChartRoute = "Chart";

    public static void Initialize()
    {
        Routing.RegisterRoute(ChartRoute, typeof(ChartPage));
    }
}