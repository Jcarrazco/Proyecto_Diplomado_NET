using CommunityToolkit.Maui.Alerts;

namespace ActivosApp.Services;

public class NotificationService
{
    public Task ShowToast(string message)
    {
        return Toast.Make(message).Show();
    }
}
