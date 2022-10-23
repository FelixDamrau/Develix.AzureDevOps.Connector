using Develix.AzureDevOps.Connector.App.Services;
using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.CreateWorkItemPageUseCase;

public class Effects
{
    private readonly IWorkItemService workItemService;
    private readonly ISnackbarService snackbarService;

    public Effects(IWorkItemService workItemService, ISnackbarService snackbarService)
    {
        this.workItemService = workItemService;
        this.snackbarService = snackbarService;
    }

    [EffectMethod]
    public async Task HandleCreateWorkItemAction(CreateWorkItemAction action, IDispatcher dispatcher)
    {
        var workItemResult = await workItemService.CreateWorkItem(action.WorkItemTemplate).ConfigureAwait(false);
        if (workItemResult.Valid)
        {
            var resultAction = new CreateWorkItemResultAction(workItemResult.Value);
            dispatcher.Dispatch(resultAction);
        }
        else
        {
            snackbarService.SendError("Could not create work item!", workItemResult.Message);
        }
    }

    [EffectMethod]
    public async Task HandleGetWorkItemTypesAction(GetWorkItemTypesAction action, IDispatcher dispatcher)
    {
        var workItemTypesResult = await workItemService.GetWorkItemTypes(action.Project).ConfigureAwait(false);
        if (workItemTypesResult.Valid)
        {
            dispatcher.Dispatch(new GetWorkItemTypesResultAction(workItemTypesResult.Value));

        }
        else
        {
            snackbarService.SendError("Could not get work item types!", workItemTypesResult.Message);
        }
    }
}
