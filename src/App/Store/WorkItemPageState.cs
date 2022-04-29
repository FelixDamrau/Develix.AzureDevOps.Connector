using Develix.AzureDevOps.Connector.Model;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store;

[FeatureState]
public record WorkItemPageState
{
    public bool IsLoading { get; init; } = false;

    public IReadOnlyList<WorkItem> WorkItems { get; init; } = new List<WorkItem>();
}
