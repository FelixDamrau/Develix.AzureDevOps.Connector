using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record GetAllPackagesAction(string Project, string Feed);

public record GetPackageAction(string Project, string Feed, string PackageName);

public record GetPackagesResultAction(IReadOnlyList<Package> Packages);
