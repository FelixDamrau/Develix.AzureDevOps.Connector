using Develix.AzureDevOps.Connector.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal class WorkItemTypeCache : Cache<WorkItemType>
{
    public WorkItemTypeCache(WorkItemTrackingHttpClient workItemTrackingHttpClient)
        : base(workItemTrackingHttpClient, WorkItemType.Unknown)
    {
    }

    protected override async Task<ProjectItemCache> CreateItemCache(string project)
    {
        var workItemTypes = await workItemTrackingHttpClient.GetWorkItemTypesAsync(project);
        var workItemTypesDictionary = workItemTypes
            .Select(tfWi => new WorkItemType(tfWi.Name, tfWi.Description, tfWi.Color, tfWi.Icon.Url))
            .ToDictionary(wi => wi.Name);
        return new(project, workItemTypesDictionary);
    }
}
