using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemService : VssService<WorkItemTrackingHttpClient, WorkItemTrackingLogin>, IWorkItemService, IDisposable
{
    private readonly IReposService reposService;

    public WorkItemService(IReposService reposService)
    {
        this.reposService = reposService;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<Model.WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests)
    {
        if (!IsInitialized())
            return Result.Fail<IReadOnlyList<Model.WorkItem>>("Service is not initialized");
        return await Wrap(() => GetWorkItemsInternal(ids, includePullRequests));
    }

    protected override async Task<WorkItemTrackingLogin> CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return await WorkItemTrackingLogin.Create(baseUri, azureDevopsWorkItemReadToken);
    }

    private async Task<IReadOnlyList<Model.WorkItem>> GetWorkItemsInternal(IEnumerable<int> ids, bool includePullRequests)
    {
        IsInitializedGuard();

        if (!ids.Any())
            return Array.Empty<Model.WorkItem>();

        var queryResult = await RunQueryAsync(azureDevopsLogin.VssClient, ids);
        var workItems = queryResult.Select(tfWi => Create(tfWi, azureDevopsLogin.AzureDevopsOrgUri, includePullRequests, azureDevopsLogin.WorkItemFactory));
        return await Task.WhenAll(workItems);
    }

    private async Task<Model.WorkItem> Create(WorkItem azureDevopsWorkItem, Uri baseOrgUri, bool includePullRequests, WorkItemFactory workItemFactory)
    {
        var workItem = await workItemFactory.Create(azureDevopsWorkItem, baseOrgUri);
        if (includePullRequests)
            await AddPullRequests(azureDevopsWorkItem, workItem);
        return workItem;
    }

    private async Task AddPullRequests(WorkItem azureDevopsWorkItem, Model.WorkItem workItem)
    {
        var prRelations = azureDevopsWorkItem.Relations?.Where(r => PullRequestFactory.IsPullRequestRelation(r)).ToList();
        if (prRelations is not null && prRelations.Count > 0)
        {
            var prIds = prRelations.Select(prr => PullRequestFactory.GetPullRequestId(prr)).Where(r => r.Valid).Select(r => r.Value);

            var gitPrs = reposService.GetPullRequests(prIds);
            await foreach (var pr in gitPrs)
            {
                workItem.AddPullRequest(pr);
            }
        }
    }

    private static async Task<IReadOnlyList<WorkItem>> RunQueryAsync(WorkItemTrackingHttpClient client, IEnumerable<int> ids)
    {
        var result = await GetExistingWorkItemsIds(client, ids);
        var existingIds = result.WorkItems.Select(wi => wi.Id).ToList();
        if (existingIds.Count == 0)
            return Array.Empty<WorkItem>();

        return await client.GetWorkItemsAsync(
            ids: existingIds,
            asOf: result.AsOf,
            expand: WorkItemExpand.Relations)
            .ConfigureAwait(false);
    }

    private static async Task<WorkItemQueryResult> GetExistingWorkItemsIds(WorkItemTrackingHttpClient client, IEnumerable<int> ids)
    {
        var wiql = new Wiql()
        {
            Query =
                "Select [Id] " +
                "From WorkItems " +
                $"Where [Id] IN ({string.Join(", ", ids)}) " +
                "Order By [Id] Desc",
        };
        return await client.QueryByWiqlAsync(wiql).ConfigureAwait(false);
    }

    #region IDisposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                azureDevopsLogin?.VssClient.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
