namespace Wibor.Services;

public interface INotifier
{
    Task NotifyException(Exception ex);
}

public class PopupNotifier : INotifier
{
    public Task NotifyException(Exception ex)
    {
        return Shell.Current.CurrentPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
    }
}