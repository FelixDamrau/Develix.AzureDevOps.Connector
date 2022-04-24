using Develix.Essentials.Core;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace Develix.AzureDevOps.Connector.Service;

public class QueryClient
{
    private readonly Uri azureDevopsOrgUri;
    private readonly string azureDevopsWorkItemReadToken;
    private readonly string azureDevopsPullRequestReadToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryClient" /> class.
    /// </summary>
    /// <param name="azureDevopsOrgUri">The URI of the azure devops organization, e.g.: https://dev.azure.com/myOrgName </param>
    /// <param name="azureDevopsWorkItemReadToken">A from the provided <paramref name="orgName"/> with at least a work item read permission.</param>
    /// <param name="azureDevopsPullRequestReadToken">A from the provided <paramref name="orgName"/> with at least a pull request read permission.</param>
    public QueryClient(Uri azureDevopsOrgUri, string azureDevopsWorkItemReadToken, string azureDevopsPullRequestReadToken)
    {
        this.azureDevopsOrgUri = azureDevopsOrgUri;
        this.azureDevopsWorkItemReadToken = azureDevopsWorkItemReadToken;
        this.azureDevopsPullRequestReadToken = azureDevopsPullRequestReadToken;
    }

    public async Task<Result<IReadOnlyList<Model.WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests)
    {
        var wiql = new Wiql()
        {
            Query = "Select [Id] " +
            "From WorkItems " +
            $"Where [Id] IN ({string.Join(", ", ids)}) " +
            "Order By [Id] Desc",
        };
        var queryResults = await RunQueryAsync(wiql);
        var workItems = new List<Model.WorkItem>();
        if (!queryResults.Valid)
            return Result.Fail<IReadOnlyList<Model.WorkItem>>(queryResults.Message);
        foreach (var azureDevopsWorkItem in queryResults.Value)
        {
            var workItem = await Create(azureDevopsWorkItem, includePullRequests);
            workItems.Add(workItem);
        }
        return Result.Ok<IReadOnlyList<Model.WorkItem>>(workItems);
    }

    private async Task<Model.WorkItem> Create(WorkItem azureDevopsWorkItem, bool includePullRequests)
    {
        var workItem = WorkItemFactory.Create(azureDevopsWorkItem, azureDevopsOrgUri);
        if (includePullRequests)
            await AddPullRequests(azureDevopsWorkItem, workItem);
        return workItem;
    }

    private async Task AddPullRequests(WorkItem azureDevopsWorkItem, Model.WorkItem workItem)
    {
        var prRelations = azureDevopsWorkItem.Relations?.Where(r => PullRequestFactory.IsPullRequestRelation(r)).ToList();
        if (prRelations is not null && prRelations.Count > 0)
        {
            var prClient = new PullRequestService();
            await prClient.Initialize(azureDevopsOrgUri, azureDevopsPullRequestReadToken);
            var prIds = prRelations.Select(prr => PullRequestFactory.GetPullRequestId(prr)).Where(r => r.Valid).Select(r => r.Value);

            var gitPrs = prClient.GetPullRequests(prIds);
            await foreach (var pr in gitPrs)
            {
                workItem.AddPullRequest(pr);
            }
        }
    }

    private async Task<Result<IReadOnlyList<WorkItem>>> RunQueryAsync(Wiql wiql)
    {
        try
        {
            var credentials = new VssBasicCredential(string.Empty, azureDevopsWorkItemReadToken);
            using var wiClient = new WorkItemTrackingHttpClient(azureDevopsOrgUri, credentials);
            var result = await wiClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
            var ids = result.WorkItems.Select(item => item.Id).ToList();

            if (ids.Count == 0)
                return Result.Ok<IReadOnlyList<WorkItem>>(Array.Empty<WorkItem>());

            return Result.Ok<IReadOnlyList<WorkItem>>(await wiClient.GetWorkItemsAsync(
                ids: ids,
                asOf: result.AsOf,
                expand: WorkItemExpand.Relations)
                .ConfigureAwait(false));
        }
        catch (VssUnauthorizedException exception)
        {
            return Result.Fail<IReadOnlyList<WorkItem>>(exception.Message);
        }
    }
}
