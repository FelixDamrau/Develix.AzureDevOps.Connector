using System.Linq.Expressions;
using Develix.AzureDevOps.Connector.Model;
using Microsoft.AspNetCore.Components;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class AreaPathSelect
{
    [Parameter]
    [EditorRequired]
    public IReadOnlyList<AreaPath> AreaPaths { get; set; } = Array.Empty<AreaPath>();

    [Parameter]
    public Expression<Func<AreaPath>>? For { get; set; }

    [Parameter]
    public AreaPath? Value { get; set; }

    [Parameter]
    public EventCallback<AreaPath?> ValueChanged { get; set; }

    private async Task SelectedAreaPathChanged(AreaPath areaPath)
    {
        await ValueChanged.InvokeAsync(areaPath).ConfigureAwait(true);
    }

    private string GetLabel()
    {
        return AreaPaths switch
        {
            null or { Count: 0 } => "No area paths loaded",
            { Count: > 0 } => "Select area path"
        };
    }

    private bool GetDisabled()
    {
        return AreaPaths switch
        {
            null or { Count: 0 } => true,
            { Count: > 0 } => false
        };
    }

    private Task<IEnumerable<AreaPath>> Search(string value)
    {
        return Task.FromResult(AreaPaths.Where(p => p.Name.Contains(value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase)));
    }
}
