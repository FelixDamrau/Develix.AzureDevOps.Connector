﻿using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

/// <summary>
/// This service connects to Azure Repos.
/// </summary>
public interface IReposService : IAzureDevOpsService
{
    /// <summary>
    /// Gets all pull requests with the given <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">The IDs of the pull requests that should be fetched.</param>
    Task<Result<IReadOnlyList<PullRequest>>> GetPullRequests(IEnumerable<int> ids);
}
