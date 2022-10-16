using Develix.AzureDevOps.Connector.App.Services;
using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.WorkItemsPageUseCase;

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
    public async Task HandleGetPullRequestsAction(GetWorkItemsAction action, IDispatcher dispatcher)
    {
        var workItemResult = await workItemService.GetWorkItems(action.Ids, false).ConfigureAwait(false);
        var workItems = workItemResult.Valid ? workItemResult.Value : Array.Empty<WorkItem>();
        var resultAction = new GetWorkItemsResultAction(workItems);
        dispatcher.Dispatch(resultAction);

        if (workItemResult.Valid)
        {
            var notFoundIds = action.Ids.Except(workItemResult.Value.Select(wi => wi.Id));
            NotifyNotFoundIds(notFoundIds);
        }
        else
        {
            snackbarService.SendError($"Could not get work items! Message: {workItemResult.Message}");
        }
    }

    private void NotifyNotFoundIds(IEnumerable<int> notFoundIds)
    {
        if (notFoundIds.Any())
        {
            var message = $"Could not find work items with IDs: {string.Join(", ", notFoundIds)}";
            snackbarService.SendWarning(message);
        }
    }
}
