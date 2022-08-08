using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;
/// <summary>
/// This service connects to Azure Feeds and gets information about packages.
/// </summary>
public interface IPackagesService : IAzureDevOpsService
{
    /// <summary>
    /// Gets all packages from the given <paramref name="feed"/> of the given <paramref name="project"/>.
    /// </summary>
    /// <param name="project">The project where the <paramref name="feed"/> is hosted.</param>
    /// <param name="feed">The feed that lists all packages to get.</param>
    public Task<Result<IReadOnlyList<Package>>> GetPackages(string project, string feed);

    /// <summary>
    /// Gets the package with the name <paramref name="packageName"/> 
    /// from the given <paramref name="feed"/> of the given <paramref name="project"/>.
    /// </summary>
    /// <param name="project">The project where the <paramref name="feed"/> is hosted.</param>
    /// <param name="feed">The feed that lists the package to get.</param>
    /// <param name="packageName">The name of the package to get.</param>
    /// <returns></returns>
    public Task<Result<Package>> GetPackage(string project, string feed, string packageName);
}
