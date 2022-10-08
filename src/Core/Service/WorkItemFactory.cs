using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemFactory
{
    private readonly WorkItemTypeCache workItemTypeCache;

    public WorkItemFactory(WorkItemTrackingHttpClient workItemTrackingHttpClient)
    {
        workItemTypeCache = new WorkItemTypeCache(workItemTrackingHttpClient);
    }

    public async Task<WorkItem> Create(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem, Uri azureDevopsOrgUri)
    {
        var status = GetStatus(workItem);
        var teamProject = GetTeamProject(workItem);
        var title = GetTitle(workItem);
        var workItemType = await GetWorkItemType(workItem);
        var azureDevopsLink = GetAzureDevopsLink(azureDevopsOrgUri, teamProject, workItem.Id);
        return new WorkItem
        {
            Id = workItem.Id ?? -1,
            AzureDevopsLink = azureDevopsLink,
            Status = status,
            TeamProject = teamProject,
            Title = title,
            WorkItemType = workItemType
        };
    }

    private static WorkItemStatus GetStatus(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
        if (workItem.Fields["System.State"] is string workItemStatusExpression)
        {
            return workItemStatusExpression.ToLowerInvariant() switch
            {
                "new" => WorkItemStatus.New,
                "approved" => WorkItemStatus.Approved,
                "committed" => WorkItemStatus.Committed,
                "done" => WorkItemStatus.Done,
                "removed" => WorkItemStatus.Removed,
                "in progress" => WorkItemStatus.InProgress,
                "open" => WorkItemStatus.Open,
                "closed" => WorkItemStatus.Closed,
                "to do" => WorkItemStatus.ToDo,
                _ => WorkItemStatus.Invalid
            };
        }
        return WorkItemStatus.Invalid;
    }

    private static string GetTeamProject(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
        if (workItem.Fields["System.TeamProject"] is string teamProject && !string.IsNullOrWhiteSpace(teamProject))
            return teamProject;
        return "No team project";
    }

    private static string GetTitle(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
        if (workItem.Fields["System.Title"] is string title && !string.IsNullOrWhiteSpace(title))
            return title;
        return "No title";
    }

    private async Task<WorkItemType> GetWorkItemType(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
        if (workItem.Fields["System.WorkItemType"] is string workItemTypeExpression
            && workItem.Fields["System.TeamProject"] is string teamProject)
        {
            return await workItemTypeCache.Get(teamProject, workItemTypeExpression);
        }
        return WorkItemType.Invalid;
    }

    private static string GetAzureDevopsLink(Uri azureDevopsOrgUri, string teamProject, int? id) => $"{azureDevopsOrgUri}{teamProject}/_workitems/edit/{id}";
}
