using Develix.AzureDevOps.Connector.App.Store;
using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Reader;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Develix.AzureDevOps.Connector.App.Pages;

public partial class WorkItems
{
    [Inject]
    private IState<AzdoConnectionState> azdoConnectionState { get; set; } = default!;
    private bool processing = false;
    private string? errorText;
    private IEnumerable<WorkItem>? workItems;
    private readonly HashSet<int> workItemIds = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        azdoConnectionState.StateChanged += AzdoConnectionState_StateChanged;
    }

    private void AzdoConnectionState_StateChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private async Task GetWorkItems()
    {
        if (!azdoConnectionState.Value.Valid())
            return;

        try
        {
            processing = true;
            var queryClient = new QueryClient(azdoConnectionState.Value.AzureDevopsOrgUri, azdoConnectionState.Value.Token, azdoConnectionState.Value.Token);
            if (workItemIds.Count <= 0)
                return;

            var queryResult = await queryClient.GetWorkItems(workItemIds, true);
            if (queryResult.Valid)
                SetValidState(queryResult.Value);
            else
                SetInvalidState(queryResult.Message);
        }
        finally
        {
            processing = false;
        }
    }

    private bool CanGetWorkItems() => !processing && azdoConnectionState.Value.Valid() && workItemIds.Count > 0;

    private void SetValidState(IReadOnlyList<WorkItem> validWorkItems)
    {
        workItems = validWorkItems;
        errorText = null;
    }

    private void SetInvalidState(string message)
    {
        workItems = null;
        errorText = message;
    }
}
