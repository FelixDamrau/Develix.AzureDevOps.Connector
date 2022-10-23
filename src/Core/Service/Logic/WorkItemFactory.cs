using Develix.AzureDevOps.Connector.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using TfWorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace Develix.AzureDevOps.Connector.Service.Logic;

public class WorkItemFactory
{
    private readonly WorkItemTypeCache workItemTypeCache;
    private readonly WorkItemStatusCache workItemStatusCache;

    public WorkItemFactory(WorkItemTrackingHttpClient workItemTrackingHttpClient)
    {
        workItemTypeCache = new WorkItemTypeCache(workItemTrackingHttpClient);
        workItemStatusCache = new WorkItemStatusCache(workItemTrackingHttpClient);
    }

    public async Task<WorkItem> Create(TfWorkItem workItem, Uri azureDevopsOrgUri)
    {
        var teamProject = GetTeamProject(workItem);
        var title = GetTitle(workItem);
        var workItemType = await GetWorkItemType(workItem).ConfigureAwait(false);
        var status = await GetStatus(workItem, teamProject, workItemType.Name).ConfigureAwait(false);
        var azureDevopsLink = GetAzureDevopsLink(azureDevopsOrgUri, teamProject, workItem.Id);
        var areaPath = GetAreaPath(workItem);
        return new WorkItem
        {
            Id = workItem.Id ?? -1,
            AzureDevopsLink = azureDevopsLink,
            AreaPath = areaPath,
            Status = status,
            TeamProject = teamProject,
            Title = title,
            WorkItemType = workItemType
        };
    }

    private async Task<WorkItemStatus> GetStatus(TfWorkItem workItem, string teamProject, string typeName)
    {
        if (workItem.Fields["System.State"] is string workItemStatusExpression)
        {
            return await workItemStatusCache.Get(new(teamProject, typeName), workItemStatusExpression).ConfigureAwait(false);
        }
        return WorkItemStatus.Invalid;
    }

    private static string GetTeamProject(TfWorkItem workItem)
    {
        if (workItem.Fields["System.TeamProject"] is string teamProject && !string.IsNullOrWhiteSpace(teamProject))
            return teamProject;
        return "No team project";
    }

    private static string GetTitle(TfWorkItem workItem)
    {
        if (workItem.Fields["System.Title"] is string title && !string.IsNullOrWhiteSpace(title))
            return title;
        return "No title";
    }

    private async Task<WorkItemType> GetWorkItemType(TfWorkItem workItem)
    {
        if (workItem.Fields["System.WorkItemType"] is string workItemTypeExpression
            && workItem.Fields["System.TeamProject"] is string teamProject)
        {
            return await workItemTypeCache.Get(new(teamProject), workItemTypeExpression).ConfigureAwait(false);
        }
        return WorkItemType.Invalid;
    }

    private static string GetAzureDevopsLink(Uri azureDevopsOrgUri, string teamProject, int? id)
        => $"{azureDevopsOrgUri}{teamProject}/_workitems/edit/{id}";

    private static AreaPath GetAreaPath(TfWorkItem workItem)
    {
        if (workItem.Fields["System.AreaPath"] is string areaPath
            && workItem.Fields["System.AreaId"] is long areaID)
        {
            return new() { Id = areaID, Name = areaPath };
        }
        return AreaPath.Invalid;
    }
}
