using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.ConnectAzureDevOpsServiceUseCase;
public class Reducers
{
    [ReducerMethod(typeof(LoginPullRequestServiceAction))]
    public static AzureDevOpsServicesState LoginPullRequestService(AzureDevOpsServicesState state)
    {
        return state with { PullRequestServiceConnectionStatus = Model.ConnectionStatus.Connecting };
    }

    [ReducerMethod]
    public static AzureDevOpsServicesState LoginPullRequestServiceResultAction(AzureDevOpsServicesState state, LoginPullRequestServiceResultAction action)
    {
        return state with { PullRequestServiceConnectionStatus = action.ConnectionStatus };
    }
}
