using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.ConnectAzureDevOpsServiceUseCase;
public class Reducers
{
    [ReducerMethod(typeof(LoginPullRequestServiceAction))]
    public static AzureDevOpsServicesState ReduceLoginPullRequestServiceAction(AzureDevOpsServicesState state)
    {
        return state with { PullRequestServiceConnectionStatus = Model.ConnectionStatus.Connecting };
    }

    [ReducerMethod]
    public static AzureDevOpsServicesState ReduceLoginPullRequestServiceResultAction(AzureDevOpsServicesState state, LoginPullRequestServiceResultAction action)
    {
        return state with { PullRequestServiceConnectionStatus = action.ConnectionStatus };
    }

    [ReducerMethod(typeof(LoginWorkItemServiceAction))]
    public static AzureDevOpsServicesState ReduceLoginWorkItemtServiceAction(AzureDevOpsServicesState state)
    {
        return state with { WorkItemServiceConnectionStatus = Model.ConnectionStatus.Connecting };
    }

    [ReducerMethod]
    public static AzureDevOpsServicesState ReduceLoginWorkItemServiceResultAction(AzureDevOpsServicesState state, LoginWorkItemServiceResultAction action)
    {
        return state with { WorkItemServiceConnectionStatus = action.ConnectionStatus };
    }
}
