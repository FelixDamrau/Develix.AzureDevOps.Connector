using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.App.Store;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class AzureDevOpsServiceLogin
{

    private Uri? azureDevopsOrgUri;
    private string? token;
    private readonly Converter<Uri> converter = new() { SetFunc = value => value?.AbsoluteUri ?? string.Empty, GetFunc = text => new Uri(text), };

    [Inject]
    [NotNull]
    private IState<AzureDevOpsServicesState>? azureServicesState { get; set; }

    [Inject]
    [NotNull]
    private IDispatcher? dispatcher { get; set; }

    [Parameter]
    [EditorRequired]
    [NotNull]
    public Func<Uri, string, ILoginServiceAction>? LoginServiceActionFactory { get; set; }

    [MemberNotNullWhen(true, new[] { nameof(token), nameof(azureDevopsOrgUri) })]
    private bool CanLogin() => token is not null && azureDevopsOrgUri is not null;

    private void Login()
    {
        if (CanLogin())
        {
            var action = LoginServiceActionFactory(azureDevopsOrgUri, token);
            dispatcher.Dispatch(action);
        }
    }
}
