using Develix.AzureDevOps.Connector.App.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record LoginPackagesServiceAction(Uri AzureDevopsOrgUri, string Token) : ILoginServiceAction;

public record LoginPackagesServiceResultAction(ConnectionStatus ConnectionStatus);
