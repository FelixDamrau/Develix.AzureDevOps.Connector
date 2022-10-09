namespace Develix.AzureDevOps.Connector.Service.Logic;

public class PackageServiceLogin : AzureDevopsLogin<PackageServiceHttpClient>
{
    private PackageServiceLogin(Uri azureDevopsOrgUri, PackageServiceHttpClient vssClient)
    : base(azureDevopsOrgUri, vssClient)
    {
    }

    public static async Task<PackageServiceLogin> Create(Uri azureDevopsOrgUri, string token)
    {
        var vssClient = new PackageServiceHttpClient(azureDevopsOrgUri.AbsoluteUri, token);
        await ConnectionCheck(vssClient);
        return new PackageServiceLogin(azureDevopsOrgUri, vssClient);
    }

    private static async Task ConnectionCheck(PackageServiceHttpClient client)
    {
        await client.ConnectionCheck();
    }
}
