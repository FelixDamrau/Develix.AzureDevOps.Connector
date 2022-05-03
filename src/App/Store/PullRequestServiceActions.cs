using Develix.AzureDevOps.Connector.App.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record LoginRepoServiceAction(Uri AzureDevopsOrgUri, string Token) : ILoginServiceAction;

public record LoginRepoServiceResultAction(ConnectionStatus ConnectionStatus);
