using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.App.Store;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class AzureDevOpsServiceStatus
{
    [Inject]
    [NotNull]
    public IState<AzureDevOpsServicesState>? azureDevOpsServicesState { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        azureDevOpsServicesState.StateChanged += AzureDevOpsServicesState_StateChanged;
    }

    private void AzureDevOpsServicesState_StateChanged(object? sender, EventArgs e) => StateHasChanged();
}
