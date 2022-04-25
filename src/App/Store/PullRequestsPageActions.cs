using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record GetPullRequestsAction(IEnumerable<int> Ids);

public record GetPullRequestsResultAction(IReadOnlyList<PullRequest> PullRequests);
