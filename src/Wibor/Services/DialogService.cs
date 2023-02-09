namespace Wibor.Services;

public interface IDialogService
{
    Task ShowException(Exception ex);
}

public class DialogService : IDialogService
{
    public Task ShowException(Exception ex)
    {
        return Shell.Current.CurrentPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
    }
}