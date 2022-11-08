using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Model.PackageServiceModel;
using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public class PackagesService : VssService<PackageServiceHttpClient, PackageServiceLogin>, IPackagesService
{
    public async Task<Result<IReadOnlyList<Package>>> GetPackages(string project, string feed)
    {
        if (!IsInitialized())
            return Result.Fail<IReadOnlyList<Package>>("Service was not initialized");

        var result = await azureDevopsLogin.VssClient.GetAllPackages(project, feed).ConfigureAwait(false);
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

        var result = await azureDevopsLogin.VssClient.GetPackage(project, feed, packageName).ConfigureAwait(false);
        if (!result.Valid)
            return Result.Fail<Package>($"Could not get package. Message: {result.Message}");

        return Result.Ok(ToPackage(result.Value));
    }

    protected override PackageServiceLogin CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return PackageServiceLogin.Create(baseUri, azureDevopsWorkItemReadToken);
    }

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

    protected override async Task CheckConnection(PackageServiceHttpClient vssClient)
    {
        await vssClient.ConnectionCheck().ConfigureAwait(false);
    }
}
