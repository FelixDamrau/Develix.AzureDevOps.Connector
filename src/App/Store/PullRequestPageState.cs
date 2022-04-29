using Develix.AzureDevOps.Connector.Model;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store;

[FeatureState]
public record PullRequestPageState
{
    public bool IsLoading { get; init; } = false;

    public IReadOnlyList<PullRequest> PullRequests { get; init; } = new List<PullRequest>();
}
