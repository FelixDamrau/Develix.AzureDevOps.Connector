using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.Service;
/// <summary>
/// This service connects to Azure Feeds and gets information about packages.
/// </summary>
public interface IPackagesService : IAzureDevOpsService
{
    /// <summary>
    /// Gets all packages.
    /// </summary>
    public IAsyncEnumerable<Package> GetPackages(string project, string feed);
}
