﻿namespace Develix.AzureDevOps.Connector.Model;
public class WorkItemCreateTemplate
{
    public WorkItemCreateTemplate(string title, string workItemType, string project)
    {
        Title = title;
        WorkItemType = workItemType;
        Project = project;
    }

    public string Title { get; init; }
    public string WorkItemType { get; init; }
    public string Project { get; init; }
    public long? AreaId { get; init; }
    public string? AssignedTo { get; init; }
    public int? ParentWorkItemId { get; init; }
}
