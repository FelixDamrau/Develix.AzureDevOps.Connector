namespace Develix.AzureDevOps.Connector.Service.Logic;

public abstract class AzureDevopsLogin<TVssClient>
    where TVssClient : class
{
    protected AzureDevopsLogin(Uri azureDevopsOrgUri, TVssClient vssClient)
    {
        AzureDevopsOrgUri = azureDevopsOrgUri;
        VssClient = vssClient;
    }

    public Uri AzureDevopsOrgUri { get; }
    public TVssClient VssClient { get; }
}
