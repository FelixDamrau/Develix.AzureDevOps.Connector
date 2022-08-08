using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record GetAllPackagesAction(string Project, string Feed);

public record GetAllPackagesResultAction(IReadOnlyList<Package> Packages, string? ErrorMessage = null);

public record GetPackageAction(string Project, string Feed, string PackageName);

public record GetPackageResultAction(Package? Package, string? ErrorMessage = null);
