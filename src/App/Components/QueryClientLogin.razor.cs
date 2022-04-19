using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.App.Store;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class QueryClientLogin
{
    private readonly Converter<Uri> converter = new() { SetFunc = value => value?.AbsoluteUri ?? string.Empty, GetFunc = text => new Uri(text), };

    private Uri? azureDevopsOrgUri;
    private string? token;
    private bool validStateDispatched = false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IDispatcher Dispatcher { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [MemberNotNullWhen(true, nameof(azureDevopsOrgUri))]
    [MemberNotNullWhen(true, nameof(token))]
    private bool IsValid() => azureDevopsOrgUri is not null && token is not null;

    private void SetAzdoConnectionState()
    {
        if (IsValid())
        {
            var action = new SetAzdoConnectionStateAction(azureDevopsOrgUri, token);
            Dispatcher.Dispatch(action);
            validStateDispatched = true;
        }
    }

    private string GetIcon() => validStateDispatched ? Icons.Material.Filled.CheckCircle : Icons.Material.Filled.MobiledataOff;
    private Color GetColor() => validStateDispatched ? Color.Success : Color.Primary;
}
