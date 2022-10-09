using Develix.AzureDevOps.Connector.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal class WorkItemTypeCache : Cache<WorkItemType, WorkItemTypeCacheKey>
{
    public WorkItemTypeCache(WorkItemTrackingHttpClient workItemTrackingHttpClient)
        : base(workItemTrackingHttpClient, WorkItemType.Unknown)
    {
    }

    protected override async Task<ItemCache> CreateItemCache(WorkItemTypeCacheKey key)
    {
        var workItemTypes = await workItemTrackingHttpClient.GetWorkItemTypesAsync(key.Project).ConfigureAwait(false);
        var workItemTypesDictionary = workItemTypes
            .Select(tfWi => new WorkItemType(tfWi.Name, tfWi.Description, tfWi.Color, tfWi.Icon.Url))
            .ToDictionary(wi => wi.Name);
        return new(workItemTypesDictionary);
    }
}
