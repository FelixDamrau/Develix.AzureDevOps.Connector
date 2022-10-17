using Develix.AzureDevOps.Connector.Model;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PackagesPageUseCase;

[FeatureState]
public record PackagesPageState
{
    public bool IsLoading { get; init; } = false;

    public IReadOnlyList<Package> Packages { get; init; } = new List<Package>();
}
