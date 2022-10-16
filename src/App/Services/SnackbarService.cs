using MudBlazor;

namespace Develix.AzureDevOps.Connector.App.Services;

public class SnackbarService : ISnackbarService
{
    private readonly ISnackbar snackbar;

    public SnackbarService(ISnackbar snackbar)
    {
        this.snackbar = snackbar;
    }

    public void SendWarning(string message) => snackbar.Add(message, Severity.Warning);

    public void SendError(string message) => snackbar.Add(message, Severity.Error);
}
