using System.Diagnostics.CodeAnalysis;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store;

[FeatureState]
public record AzdoConnectionState
{
    private AzdoConnectionState()
    {
        // needed for initial state creation
    }

    public AzdoConnectionState(Uri azureDevopsOrgUri, string token)
    {
        AzureDevopsOrgUri = azureDevopsOrgUri;
        Token = token;
    }

    public Uri? AzureDevopsOrgUri { get; init; }
    public string? Token { get; init; }

    [MemberNotNullWhen(true, nameof(AzureDevopsOrgUri), nameof(Token))]
    public bool Valid() => AzureDevopsOrgUri is not null && Token is not null;

}
