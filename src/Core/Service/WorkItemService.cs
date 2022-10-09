using System.Diagnostics.CodeAnalysis;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemService : IWorkItemService, IDisposable
{
    private readonly IReposService reposService;

    private Uri? azureDevopsOrgUri;
    private ServiceState state;
    private WorkItemFactory? workItemFactory;
    private WorkItemTrackingHttpClient? workItemTrackingHttpClient;

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

    /// <inheritdoc/>
    public async Task<Result> Initialize(Uri azureDevopsOrgUri, string azureDevopsWorkItemReadToken)
    {
        state = ServiceState.NotInitialized;
        var result = await Wrap(() => InitializeInternal(azureDevopsOrgUri, azureDevopsWorkItemReadToken));
        if (result.Valid)
        {
            state = ServiceState.Initialized;
            return Result.Ok();
        }
        state = ServiceState.InitializationFailed;
        return Result.Fail(result.Message);
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(workItemTrackingHttpClient), nameof(azureDevopsOrgUri), nameof(workItemFactory))]
    public bool IsInitialized()
    {
        return state == ServiceState.Initialized
            && workItemTrackingHttpClient is not null
            && azureDevopsOrgUri is not null
            && workItemFactory is not null;
    }

    private async Task<IReadOnlyList<Model.WorkItem>> GetWorkItemsInternal(IEnumerable<int> ids, bool includePullRequests)
    {
        IsInitializedGuard();

        if (!ids.Any())
            return Array.Empty<Model.WorkItem>();

        var queryResult = await RunQueryAsync(workItemTrackingHttpClient, ids);
        var workItems = queryResult.Select(tfWi => Create(tfWi, azureDevopsOrgUri, includePullRequests, workItemFactory));
        return await Task.WhenAll(workItems);
    }

    private async Task<bool> InitializeInternal(Uri azureDevopsOrgUri, string azureDevopsWorkItemReadToken)
    {
        this.azureDevopsOrgUri = azureDevopsOrgUri;
        var credential = new VssBasicCredential(string.Empty, azureDevopsWorkItemReadToken);
        workItemTrackingHttpClient = new WorkItemTrackingHttpClient(azureDevopsOrgUri, credential);

        // Perform a simple call to check if the connection is valid
        _ = await GetExistingWorkItemsIds(workItemTrackingHttpClient, new[] { 367 });
        workItemFactory = new WorkItemFactory(workItemTrackingHttpClient);
        return true;
    }

    [MemberNotNull(nameof(workItemTrackingHttpClient), nameof(azureDevopsOrgUri), nameof(workItemFactory))]
    private void IsInitializedGuard()
    {
        if (!IsInitialized())
            throw new InvalidOperationException($"The services are not initialzed!");
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

    private async Task<Result<T>> Wrap<T>(Func<Task<T>> action)
    {
        try
        {
            var response = await action.Invoke();
            return Result.Ok(response);
        }
        catch (VssUnauthorizedException e)
        {
            return Result.Fail<T>("Authorization failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (VssServiceResponseException e)
        {
            return Result.Fail<T>("Connection failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (Exception e)
        {
            return Result.Fail<T>("Unknown error!" + Environment.NewLine + "Error message: " + e.Message);
        }
    }

    #region IDisposable
    private bool disposedValue;
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
