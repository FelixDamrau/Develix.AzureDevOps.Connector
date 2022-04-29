using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.GetWorkItemsUseCase;

public static class Reducers
{
    [ReducerMethod(typeof(GetWorkItemsAction))]
    public static WorkItemPageState ReduceGetWorkItemsAction(WorkItemPageState state)
    {
        return state with { IsLoading = true };
    }

    [ReducerMethod]
    public static WorkItemPageState ReduceGetWorkItemsResultAction(WorkItemPageState state, GetWorkItemsResultAction action)
    {
        return state with { IsLoading = false, WorkItems = action.WorkItems };
    }
}
