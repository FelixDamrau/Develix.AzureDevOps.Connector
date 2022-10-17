namespace Develix.AzureDevOps.Connector.App.Services;

public interface ISnackbarService
{
    void SendError(string title, string message);
    void SendWarning(string title, string message);
}
