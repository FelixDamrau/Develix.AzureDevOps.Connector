namespace Develix.AzureDevOps.Connector.App.Store;

public interface ILoginServiceAction
{
    Uri AzureDevopsOrgUri { get; }
    string Token { get; }
}
