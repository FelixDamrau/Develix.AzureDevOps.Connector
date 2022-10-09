using Develix.AzureDevOps.Connector.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal class WorkItemStatusCache : Cache<WorkItemStatus, WorkItemStatusCacheKey>
{
    public WorkItemStatusCache(WorkItemTrackingHttpClient workItemTrackingHttpClient)
        : base(workItemTrackingHttpClient, WorkItemStatus.Unknown)
    {
    }

    protected override async Task<ItemCache> CreateItemCache(WorkItemStatusCacheKey key)
    {
        var workItemStatus = await workItemTrackingHttpClient.GetWorkItemTypeStatesAsync(key.Project, key.WorkItemType).ConfigureAwait(false);
        var cache = workItemStatus
            .Select(tfWis => new WorkItemStatus(tfWis.Name, tfWis.Color, tfWis.Category))
            .ToDictionary(wis => wis.Name);
        return new(cache);
    }
}
