using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service;
using Develix.Essentials.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class CreateWorkItemForm
{
    [Inject]
    [NotNull]
    private IWorkItemService? workItemService { get; set; }

    private readonly Model model = new();

    private IReadOnlyList<WorkItemType> workItemTypes = Array.Empty<WorkItemType>();
    private IReadOnlyList<AreaPath> areaPaths = Array.Empty<AreaPath>();
    private Result<WorkItem>? createdWorkItem;

    private async Task HandleProjectFieldDebounced(string value)
    {
        var workItemTypesResult = await workItemService.GetWorkItemTypes(value).ConfigureAwait(true);
        workItemTypes = workItemTypesResult.Valid ? workItemTypesResult.Value : Array.Empty<WorkItemType>();
        var areaPathsResult = await workItemService.GetAreaPaths(value, 3).ConfigureAwait(true);
        areaPaths = areaPathsResult.Valid ? areaPathsResult.Value : Array.Empty<AreaPath>();
    }

    private async Task OnValidSubmit(EditContext context)
    {
        StateHasChanged();
        var template = new WorkItemCreateTemplate(model.Title, model.WorkItemType.Name, model.Project)
        { AreaId = model.AreaPath?.Id, AssignedTo = model.AssignedTo, ParentWorkItemId = model.ParentWorkItemId, Project = model.Project, Title = model.Title, WorkItemType = model.WorkItemType.Name, };
        var createdId = await workItemService.CreateWorkItem(template).ConfigureAwait(true);
        if (createdId.Valid)
        {
            var result = await workItemService.GetWorkItems(new[] { createdId.Value }, false).ConfigureAwait(true);
            if (result.Valid)
            {
                var wi = result.Value.FirstOrDefault();
                if (wi is not null)
                    createdWorkItem = Result.Ok(wi);
                else
                    createdWorkItem = Result.Fail<WorkItem>($"Could not find created work item");
            }
            else
            {
                createdWorkItem = Result.Fail<WorkItem>(result.Message);
            }
        }
        else
        {
            createdWorkItem = Result.Fail<WorkItem>(createdId.Message);
        }
    }

    private class Model
    {
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public WorkItemType WorkItemType { get; set; } = WorkItemType.Invalid;
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string Project { get; set; } = string.Empty;
        public AreaPath? AreaPath { get; set; }

        public string? AssignedTo { get; set; }

        public int? ParentWorkItemId { get; set; }
    }
}
