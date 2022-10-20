using Develix.AzureDevOps.Connector.Model;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.CreateWorkItemPageUseCase;

public static class Reducers
{
    [ReducerMethod(typeof(CreateWorkItemAction))]
    public static CreateWorkItemPageState ReduceCreateWorkItemAction(CreateWorkItemPageState state)
    {
        return state with
        {
            IsLoading = true,
        };
    }

    [ReducerMethod(typeof(CreateWorkItemResultAction))]
    public static CreateWorkItemPageState ReduceCreateWorkItemResultAction(CreateWorkItemPageState state)
    {
        return state with
        {
            IsLoading = false,
        };
    }

    [ReducerMethod(typeof(GetWorkItemTypesAction))]
    public static CreateWorkItemPageState ReduceGetWorkItemTypesAction(CreateWorkItemPageState state)
    {
        return state with
        {
            LoadingWorkItemTypes = true,
            WorkItemTypes = Array.Empty<WorkItemType>()
        };
    }

    [ReducerMethod]
    public static CreateWorkItemPageState ReduceGetWorkItemTypesResultAction(CreateWorkItemPageState state, GetWorkItemTypesResultAction action)
    {
        return state with
        {
            LoadingWorkItemTypes = false,
            WorkItemTypes = action.WorkItemTypes,
        };
    }
}
