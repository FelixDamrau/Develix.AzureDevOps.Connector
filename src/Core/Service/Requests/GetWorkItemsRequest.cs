using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal class GetWorkItemsRequest : WorkItemTrackingRequestBase<IReadOnlyList<Model.WorkItem>>
{
    private readonly IReposService reposService;
    private readonly IEnumerable<int> ids;
    private readonly bool includePullRequests;

    public GetWorkItemsRequest(WorkItemTrackingLogin? login, IReposService reposService, IEnumerable<int> ids, bool includePullRequests)
        : base(login)
    {
        this.reposService = reposService ?? throw new ArgumentNullException(nameof(reposService));
        this.ids = ids ?? throw new ArgumentNullException(nameof(ids));
        this.includePullRequests = includePullRequests;
    }

    protected override async Task<IReadOnlyList<Model.WorkItem>> Execute(WorkItemTrackingLogin login, CancellationToken cancellationToken = default)
    {
        if (!ids.Any())
            return Array.Empty<Model.WorkItem>();

        var queryResult = await RunQueryAsync(login).ConfigureAwait(false);
        var workItems = queryResult
            .Select(azdoWorkItem => Create(azdoWorkItem, login.AzureDevopsOrgUri, includePullRequests, login.WorkItemFactory));
        return await Task.WhenAll(workItems).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<WorkItem>> RunQueryAsync(WorkItemTrackingLogin login)
    {
        var result = await GetExistingWorkItemsIds(login).ConfigureAwait(false);
        var existingIds = result.WorkItems.Select(wi => wi.Id).ToList();
        if (existingIds.Count == 0)
            return Array.Empty<WorkItem>();

        return await login.VssClient
            .GetWorkItemsAsync(
                ids: existingIds,
                asOf: result.AsOf,
                expand: WorkItemExpand.All)
            .ConfigureAwait(false);
    }

    private async Task<Model.WorkItem> Create(WorkItem azdoWorkItem, Uri baseOrgUri, bool includePullRequests, WorkItemFactory workItemFactory)
    {
        var workItem = await workItemFactory.Create(azdoWorkItem, baseOrgUri).ConfigureAwait(false);
        if (includePullRequests)
            await AddPullRequests(azdoWorkItem, workItem).ConfigureAwait(false);
        return workItem;
    }

    private async Task<WorkItemQueryResult> GetExistingWorkItemsIds(WorkItemTrackingLogin login)
    {
        var wiql = new Wiql()
        {
            Query = $"""
                Select [Id]
                From WorkItems
                Where [Id] IN ({string.Join(", ", ids)})
                Order By [Id] Desc
                """,
        };
        return await login.VssClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
    }

    private async Task AddPullRequests(WorkItem azureDevopsWorkItem, Model.WorkItem workItem)
    {
        var prRelations = azureDevopsWorkItem.Relations?.Where(r => PullRequestFactory.IsPullRequestRelation(r)).ToList();
        if (prRelations is not null && prRelations.Count > 0)
        {
            var prIds = prRelations.Select(prr => PullRequestFactory.GetPullRequestId(prr)).Where(r => r.Valid).Select(r => r.Value);

            var pullRequestResults = await reposService.GetPullRequests(prIds).ConfigureAwait(false);
            if (pullRequestResults.Valid)
            {
                foreach (var pullRequest in pullRequestResults.Value)
                {
                    workItem.AddPullRequest(pullRequest);
                }
            }
        }
    }
}
