using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public partial class PackagesService : IPackagesService
{
    private ServiceState state = ServiceState.NotInitialized;
    private PackageServiceHttpClient? packageServiceHttpClient;

    public async IAsyncEnumerable<Package> GetPackages(string project, string feed)
    {
        if (!IsInitialized())
            yield break;

        var responseObject = await packageServiceHttpClient.GetAllPackages(project, feed);
        foreach (var package in responseObject.value)
        {
            yield return new Package()
            {
                Id = package.id,
                Name = package.name,
                Versions = package.versions
                    .Select(v => new PackageVersion
                    {
                        Version = v.normalizedVersion,
                        PublishDate = v.publishDate
                    })
                    .ToArray(),
            };
        }
    }

    public async Task<Result> Initialize(Uri azureDevopsOrgUri, string token)
    {
        packageServiceHttpClient = new PackageServiceHttpClient(azureDevopsOrgUri.AbsoluteUri, token);
        var result = await packageServiceHttpClient.IsInitialized();
        state = result.Valid ? ServiceState.Initialized : ServiceState.InitializationFailed;
        
        return Result.Ok();
    }

    [MemberNotNullWhen(true, nameof(packageServiceHttpClient))]
    public bool IsInitialized() => state == ServiceState.Initialized && packageServiceHttpClient is not null;
}
