using System.Linq.Expressions;
using Develix.AzureDevOps.Connector.Model;
using Microsoft.AspNetCore.Components;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class WorkItemTypeSelect
{
    [Parameter]
    [EditorRequired]
    public IReadOnlyList<WorkItemType>? WorkItemTypes { get; set; }

    [Parameter]
    public Expression<Func<WorkItemType>>? For { get; set; }

    [Parameter]
    public WorkItemType? Value { get; set; }

    [Parameter]
    public EventCallback<WorkItemType?> ValueChanged { get; set; }

    private async Task SelectedWorkItemsChanged(IEnumerable<WorkItemType> selectedWorkItemTypes)
    {
        var selectedWorkItem = selectedWorkItemTypes.FirstOrDefault();
        await ValueChanged.InvokeAsync(selectedWorkItem).ConfigureAwait(true);
    }

    private string GetLabel()
    {
        return WorkItemTypes switch
        {
            null or { Count: 0 } => "No work item types loaded",
            { Count: > 0 } => "Select work item type"
        };
    }

    private bool GetDisabled()
    {
        return WorkItemTypes switch
        {
            null or { Count: 0 } => true,
            { Count: > 0 } => false
        };
    }
}
