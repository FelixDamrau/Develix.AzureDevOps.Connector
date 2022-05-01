using System.Diagnostics.CodeAnalysis;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemService : IWorkItemService, IDisposable
{
    private Uri? azureDevopsOrgUri;
    private string? azureDevopsWorkItemReadToken;
    private WorkItemTrackingHttpClient? workItemTrackingHttpClient;
    private ServiceState state { get; set; }
    private bool disposedValue;
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

        var queryResults = await RunQueryAsync(workItemTrackingHttpClient, ids);
        var workItems = new List<Model.WorkItem>();
        if (!queryResults.Valid)
            return Result.Fail<IReadOnlyList<Model.WorkItem>>(queryResults.Message);
        foreach (var azureDevopsWorkItem in queryResults.Value)
        {
            var workItem = await Create(azureDevopsWorkItem, azureDevopsOrgUri, includePullRequests);
            workItems.Add(workItem);
        }
        return Result.Ok<IReadOnlyList<Model.WorkItem>>(workItems);
    }

    /// <inheritdoc/>
    public async Task<Result> Initialize(Uri azureDevopsOrgUri, string azureDevopsWorkItemReadToken)
    {
        this.azureDevopsOrgUri = azureDevopsOrgUri;
        this.azureDevopsWorkItemReadToken = azureDevopsWorkItemReadToken;
        var serviceState = await Login();
        return serviceState.Valid ? Result.Ok() : Result.Fail(serviceState.Message);
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(workItemTrackingHttpClient), nameof(azureDevopsOrgUri))]
    public bool IsInitialized() => state == ServiceState.Initialized && workItemTrackingHttpClient is not null;

    private async Task<Model.WorkItem> Create(WorkItem azureDevopsWorkItem, Uri baseOrgUri, bool includePullRequests)
    {
        var workItem = WorkItemFactory.Create(azureDevopsWorkItem, baseOrgUri);
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

    private static async Task<Result<IReadOnlyList<WorkItem>>> RunQueryAsync(WorkItemTrackingHttpClient client, IEnumerable<int> ids)
    {
        try
        {
            var result = await GetExistingWorkItemsIds(client, ids);
            var existingIds = result.WorkItems.Select(wi => wi.Id).ToList();
            if (existingIds.Count == 0)
                return Result.Ok<IReadOnlyList<WorkItem>>(Array.Empty<WorkItem>());

            var workItems = await client.GetWorkItemsAsync(
                ids: ids,
                asOf: result.AsOf,
                expand: WorkItemExpand.Relations)
                .ConfigureAwait(false);

            return Result.Ok<IReadOnlyList<WorkItem>>(workItems);
        }
        catch (Exception e)
        {
            return Result.Fail<IReadOnlyList<WorkItem>>(e.Message);
        }
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

    private async Task<Result<ServiceState>> Login()
    {
        try
        {
            var credential = new VssBasicCredential(string.Empty, azureDevopsWorkItemReadToken);
            workItemTrackingHttpClient = new WorkItemTrackingHttpClient(azureDevopsOrgUri, credential);
            _ = await GetExistingWorkItemsIds(workItemTrackingHttpClient, new[] { 367 }); // Perform a simple call to check if the connection is valid

            state = ServiceState.Initialized;
            return Result.Ok(ServiceState.Initialized);
        }
        catch (VssUnauthorizedException e)
        {
            return Error("Authorization failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (VssServiceResponseException e)
        {
            return Error("Connection failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (Exception e)
        {
            return Error("Unknown error!" + Environment.NewLine + "Error message: " + e.Message);
        }

        Result<ServiceState> Error(string message)
        {
            state = ServiceState.InitializationFailed;
            return Result.Fail<ServiceState>(message);
        }
    }

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                workItemTrackingHttpClient?.Dispose();
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
