using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal static class WorkItemTemplateFactory
{
    public static JsonPatchDocument Create(WorkItemCreateTemplate template, string azureDevopsOrgUri)
    {
        var workItem = new JsonPatchDocument();
        if (TryGetTitle(template, out var title))
            workItem.Add(title);
        if (TryGetAreaPath(template, out var areaPath))
            workItem.Add(areaPath);
        if (TryGetAssignedTo(template, out var assignedTo))
            workItem.Add(assignedTo);
        if (TryGetParentRelation(template, azureDevopsOrgUri, out var parentRelation))
            workItem.Add(parentRelation);

        return workItem;
    }

    private static bool TryGetTitle(WorkItemCreateTemplate template, out JsonPatchOperation title)
    {
        title = new()
        {
            Operation = Operation.Add,
            Path = "/fields/System.Title",
            Value = template.Title,
        };
        return true;
    }

    private static bool TryGetAreaPath(
        WorkItemCreateTemplate template,
        [NotNullWhen(true)] out JsonPatchOperation? areaPath)
    {
        if (template.AreaId is null)
        {
            areaPath = null;
            return false;
        }
        areaPath = new()
        {
            Operation = Operation.Add,
            Path = "/fields/System.AreaId",
            Value = template.AreaId,
        };
        return true;
    }

    private static bool TryGetAssignedTo(
        WorkItemCreateTemplate template,
        [NotNullWhen(true)] out JsonPatchOperation? assignedTo)
    {
        if (template.AssignedTo is null)
        {
            assignedTo = null;
            return false;
        }
        assignedTo = new()
        {
            Operation = Operation.Add,
            Path = "/fields/System.AssignedTo",
            Value = template.AssignedTo,
        };
        return true;
    }

    private static bool TryGetParentRelation(
        WorkItemCreateTemplate template,
        string azureDevopsOrgUri,
        [NotNullWhen(true)] out JsonPatchOperation? parentRelation)
    {
        if (template.ParentWorkItemId is null)
        {
            parentRelation = null;
            return false;
        }
        parentRelation = new()
        {
            Operation = Operation.Add,
            Path = "/relations/-",
            Value = new WorkItemRelation()
            {
                Rel = "System.LinkTypes.Hierarchy-Reverse",
                Url = $"{azureDevopsOrgUri}/_apis/wit/workItems/{template.ParentWorkItemId}",
            }
        };
        return true;
    }
}
