using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.Service;
public interface IPullRequestService : IAzureDevOpsService
{
    /// <summary>
    /// Gets all pull requests with the given <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">The IDs of the pull requests that should be fetched.</param>
    IAsyncEnumerable<PullRequest> GetPullRequests(IEnumerable<int> ids);
}
