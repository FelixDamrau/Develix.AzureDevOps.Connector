using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.Reader;

public static class WorkItemFactory
{
    public static WorkItem Create(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem, Uri azureDevopsOrgUri)
    {
        var status = GetStatus(workItem);
        var teamProject = GetTeamProject(workItem);
        var title = GetTitle(workItem);
        var workItemType = GetWorkItemKind(workItem);
        var azureDevopsLink = GetAzureDevopsLink(azureDevopsOrgUri.AbsoluteUri, teamProject, workItem.Id);
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
        if (workItem.Fields["System.TeamProject"] is string title && !string.IsNullOrWhiteSpace(title))
            return title;
        return "No team project";
    }

    private static string GetTitle(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
        if (workItem.Fields["System.Title"] is string title && !string.IsNullOrWhiteSpace(title))
            return title;
        return "No title";
    }

    private static WorkItemType GetWorkItemKind(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
        if (workItem.Fields["System.WorkItemType"] is string workItemTypeExpression)
        {
            return workItemTypeExpression.ToLowerInvariant() switch
            {
                "bug" => WorkItemType.Bug,
                "epic" => WorkItemType.Epic,
                "feature" => WorkItemType.Feature,
                "impediment" => WorkItemType.Impediment,
                "product backlog item" => WorkItemType.ProductBacklogItem,
                "task" => WorkItemType.Task,
                _ => WorkItemType.Unknown
            };
        }
        return WorkItemType.Unknown;
    }

    private static string GetAzureDevopsLink(string azureDevopsOrgUri, string teamProject, int? id) => $"{azureDevopsOrgUri}{teamProject}/_workitems/edit/{id}";
}
