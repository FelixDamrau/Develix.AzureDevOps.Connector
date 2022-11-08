namespace Develix.AzureDevOps.Connector.Service.Logic;

public class PackageServiceLogin : AzureDevopsLogin<PackageServiceHttpClient>
{
    private PackageServiceLogin(Uri azureDevopsOrgUri, PackageServiceHttpClient vssClient)
    : base(azureDevopsOrgUri, vssClient)
    {
    }

    public static PackageServiceLogin Create(Uri azureDevopsOrgUri, string token)
    {
        var vssClient = new PackageServiceHttpClient(azureDevopsOrgUri.AbsoluteUri, token);
        return new PackageServiceLogin(azureDevopsOrgUri, vssClient);
    }


}
