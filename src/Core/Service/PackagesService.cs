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

        var responseObjectResult = await packageServiceHttpClient.GetAllPackages(project, feed);
        if (!responseObjectResult.Valid)
            return Result.Fail<IReadOnlyList<Package>>($"Could not get packages. Message: {responseObjectResult.Message}");

        var packages = responseObjectResult.Value.value
            .Select(p =>
                new Package()
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
                })
            .ToList();

        return Result.Ok<IReadOnlyList<Package>>(packages);
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
}
