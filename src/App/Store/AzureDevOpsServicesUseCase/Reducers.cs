using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.AzureDevOpsServicesUseCase;

public class Reducers
{
    [ReducerMethod(typeof(LoginRepoServiceAction))]
    public static AzureDevOpsServicesState ReduceLoginRepoServiceAction(AzureDevOpsServicesState state)
    {
        return state with { ReposServiceConnectionStatus = Model.ConnectionStatus.Connecting };
    }

    [ReducerMethod]
    public static AzureDevOpsServicesState ReduceLoginRepoServiceResultAction(AzureDevOpsServicesState state, LoginRepoServiceResultAction action)
    {
        return state with { ReposServiceConnectionStatus = action.ConnectionStatus };
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
