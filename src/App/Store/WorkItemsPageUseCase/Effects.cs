using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.WorkItemsPageUseCase;

public class Effects
{
    private readonly IWorkItemService workItemService;

    public Effects(IWorkItemService workItemService)
    {
        this.workItemService = workItemService;
    }

    [EffectMethod]
    public async Task HandleGetPullRequestsAction(GetWorkItemsAction action, IDispatcher dispatcher)
    {
        var workItemResult = await workItemService.GetWorkItems(action.Ids, false);
        var workItems = workItemResult.Valid ? workItemResult.Value : Array.Empty<WorkItem>(); // TODO Error handling!
        var resultAction = new GetWorkItemsResultAction(workItems);
        dispatcher.Dispatch(resultAction);
    }
}
