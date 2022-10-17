using MudBlazor;

namespace Develix.AzureDevOps.Connector.App.Services;

public class SnackbarService : ISnackbarService
{
    private readonly ISnackbar snackbar;

    public SnackbarService(ISnackbar snackbar)
    {
        this.snackbar = snackbar;
    }

    public void SendWarning(string title, string message) => Add(title, message, Severity.Warning);

    public void SendError(string title, string message) => Add(title, message, Severity.Error);

    private void Add(string title, string message, Severity severity)
    {
        var snackbarMessage = $"""
            <div style="font-weight: 800">{title}</div>
            <div>{message}</div>
            """;
        snackbar.Add(snackbarMessage, severity);
    }
}
