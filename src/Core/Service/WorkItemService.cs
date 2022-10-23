using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemService : VssService<WorkItemTrackingHttpClient, WorkItemTrackingLogin>, IWorkItemService
{
    private readonly IReposService reposService;

    public WorkItemService(IReposService reposService)
    {
        this.reposService = reposService;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<Model.WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests)
    {
        return IsInitialized()
            ? await Wrap(() => GetWorkItemsInternal(ids, includePullRequests)).ConfigureAwait(false)
            : Result.Fail<IReadOnlyList<Model.WorkItem>>("Service is not initialized");
    }

    /// <inheritdoc/>
    public async Task<Result<Model.WorkItem>> CreateWorkItem(Model.WorkItemCreateTemplate template)
    {
        return IsInitialized()
            ? await Wrap(() => CreateWorkItemInternal(template)).ConfigureAwait(false)
            : Result.Fail<Model.WorkItem>("Service is not initialized");
    }

    public async Task<Result<IReadOnlyList<Model.WorkItemType>>> GetWorkItemTypes(string project)
    {
        return IsInitialized()
            ? await Wrap(() => GetWorkItemTypesInternal(project)).ConfigureAwait(false)
            : Result.Fail<IReadOnlyList<Model.WorkItemType>>("Service is not initialized");

    }

    public async Task<Result<IReadOnlyList<string>>> GetAreaPaths(string project, int depth)
    {
        return IsInitialized()
            ? await Wrap(() => GetWorkAreaPathsInternal(project, depth)).ConfigureAwait(false)
            : Result.Fail<IReadOnlyList<string>>("Service is not initialized");
    }

    protected override async Task<WorkItemTrackingLogin> CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return await WorkItemTrackingLogin.Create(baseUri, azureDevopsWorkItemReadToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<Model.WorkItem>> GetWorkItemsInternal(IEnumerable<int> ids, bool includePullRequests)
    {
        IsInitializedGuard();

        if (!ids.Any())
            return Array.Empty<Model.WorkItem>();

        var queryResult = await RunQueryAsync(azureDevopsLogin.VssClient, ids).ConfigureAwait(false);
        var workItems = queryResult
            .Select(tfWi => Create(tfWi, azureDevopsLogin.AzureDevopsOrgUri, includePullRequests, azureDevopsLogin.WorkItemFactory));
        return await Task.WhenAll(workItems).ConfigureAwait(false);
    }

    private async Task<Model.WorkItem> CreateWorkItemInternal(Model.WorkItemCreateTemplate template)
    {
        IsInitializedGuard();

        var azdoWorkItem = await azureDevopsLogin.VssClient.CreateWorkItemAsync(
            WorkItemTemplateFactory.Create(template, azureDevopsLogin.AzureDevopsOrgUri.AbsoluteUri),
            template.Project,
            template.WorkItemType,
            validateOnly: false,
            bypassRules: true,
            userState: false,
            CancellationToken.None)
            .ConfigureAwait(false);
        return await Create(azdoWorkItem, azureDevopsLogin.AzureDevopsOrgUri, false, azureDevopsLogin.WorkItemFactory)
            .ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<Model.WorkItemType>> GetWorkItemTypesInternal(string project)
    {
        IsInitializedGuard();

        var azdoWorkItemTypes = await azureDevopsLogin.VssClient
            .GetWorkItemTypesAsync(project).ConfigureAwait(false);
        return azdoWorkItemTypes
            .Select(t => new Model.WorkItemType(t.Name, t.Description, t.Color, t.Icon.Url))
            .ToList();
    }

    private async Task<IReadOnlyList<string>> GetWorkAreaPathsInternal(string project, int depth)
    {
        var areaPaths = await azureDevopsLogin.VssClient
            .GetClassificationNodeAsync(project, TreeStructureGroup.Areas, depth: depth)
            .ConfigureAwait(false);
        return areaPaths.Children.Select(x => x.Path).ToList();
    }

    private async Task<Model.WorkItem> Create(WorkItem azureDevopsWorkItem, Uri baseOrgUri, bool includePullRequests, WorkItemFactory workItemFactory)
    {
        var workItem = await workItemFactory.Create(azureDevopsWorkItem, baseOrgUri).ConfigureAwait(false);
        if (includePullRequests)
            await AddPullRequests(azureDevopsWorkItem, workItem).ConfigureAwait(false);
        return workItem;
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

    private static async Task<IReadOnlyList<WorkItem>> RunQueryAsync(WorkItemTrackingHttpClient client, IEnumerable<int> ids)
    {
        var result = await GetExistingWorkItemsIds(client, ids).ConfigureAwait(false);
        var existingIds = result.WorkItems.Select(wi => wi.Id).ToList();
        if (existingIds.Count == 0)
            return Array.Empty<WorkItem>();

        return await client.GetWorkItemsAsync(
            ids: existingIds,
            asOf: result.AsOf,
            expand: WorkItemExpand.None)
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
}
