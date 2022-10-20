using Develix.AzureDevOps.Connector.Model;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.CreateWorkItemPageUseCase;

[FeatureState]
public record CreateWorkItemPageState
{
    public bool IsLoading { get; init; } = false;
    public bool LoadingWorkItemTypes { get; init; } = false;
    public IReadOnlyList<WorkItemType> WorkItemTypes { get; init; } = Array.Empty<WorkItemType>();
}
