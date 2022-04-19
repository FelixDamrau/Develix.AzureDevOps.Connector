using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.AzdoConnectionUseCase;
public static class Reducers
{
    [ReducerMethod]
    public static AzdoConnectionState SetAzdoConnectionState(AzdoConnectionState azdoConnectionState, SetAzdoConnectionStateAction action)
    {
        return azdoConnectionState with { AzureDevopsOrgUri = action.AzureDevopsOrgUri, Token = action.Token };
    }
}
