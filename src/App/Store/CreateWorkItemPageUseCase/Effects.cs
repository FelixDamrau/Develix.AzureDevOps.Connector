using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.CreateWorkItemPageUseCase;

public class Effects
{
    private readonly IWorkItemService workItemService;


    public Effects(IWorkItemService workItemService)
    {
        this.workItemService = workItemService;
    }

    [EffectMethod]
    public async Task HandleCreateWorkItemAction(CreateWorkItemAction action, IDispatcher dispatcher)
    {
        await workItemService.CreateWorkItem(action.WorkItemType).ConfigureAwait(false);

        var resultAction = new CreateWorkItemResultAction();
        dispatcher.Dispatch(resultAction);
    }

    [EffectMethod]
    public async Task HandleGetWorkItemTypesAction(GetWorkItemTypesAction action, IDispatcher dispatcher)
    {
        var workItemTypes = await workItemService.GetWorkItemTypes(action.Project).ConfigureAwait(false);
        dispatcher.Dispatch(new GetWorkItemTypesResultAction(workItemTypes));
    }
}
