namespace Wibor.Services;

public interface INavigationService
{
    Task Navigate<TParam>(string route, TParam parameter);
}

public class NavigationService : INavigationService
{
    public Task Navigate<TParam>(string route, TParam parameter)
    {
        var navigationParameters = new Dictionary<string, object>
        {
            { typeof(TParam).Name, parameter }
        };
        return Shell.Current.GoToAsync(route, navigationParameters);
    }
}