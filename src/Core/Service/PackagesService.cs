using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public partial class PackagesService : IPackagesService
{
    private ServiceState state = ServiceState.NotInitialized;
    private PackageServiceHttpClient? packageServiceHttpClient;

    public async Task<Result<IReadOnlyList<Package>>> GetPackages(string project, string feed)
    {
        if (!IsInitialized())
            return Result.Fail<IReadOnlyList<Package>>("Service was not initialized");

        var result = await packageServiceHttpClient.GetAllPackages(project, feed);
        if (!result.Valid)
            return Result.Fail<IReadOnlyList<Package>>($"Could not get packages. Message: {result.Message}");

        var packages = result.Value.value
            .Select(p => ToPackage(p))
            .ToList();

        return Result.Ok<IReadOnlyList<Package>>(packages);
    }

    public async Task<Result<Package>> GetPackage(string project, string feed, string packageName)
    {
        if (!IsInitialized())
            return Result.Fail<Package>("Service was not initialized");

        var result = await packageServiceHttpClient.GetPackage(project, feed, packageName);
        if (!result.Valid)
            return Result.Fail<Package>($"Could not get package. Message: {result.Message}");

        return Result.Ok(ToPackage(result.Value));
    }

    public async Task<Result> Initialize(Uri azureDevopsOrgUri, string token)
    {
        packageServiceHttpClient = new PackageServiceHttpClient(azureDevopsOrgUri.AbsoluteUri, token);
        var result = await packageServiceHttpClient.IsInitialized();
        state = result.Valid ? ServiceState.Initialized : ServiceState.InitializationFailed;

        return result;
    }

    [MemberNotNullWhen(true, nameof(packageServiceHttpClient))]
    public bool IsInitialized() => state == ServiceState.Initialized && packageServiceHttpClient is not null;

    private static Package ToPackage(Value p)
    {
        return new()
        {
            Id = p.id,
            Name = p.name,
            Versions = p.versions
                .Select(v => new PackageVersion
                {
                    Version = v.normalizedVersion,
                    PublishDate = v.publishDate
                })
                .ToArray(),
        };
    }
}
