namespace Develix.AzureDevOps.Connector.App.Services;

public interface ISnackbarService
{
    void SendError(string message);
    void SendWarning(string message);
}