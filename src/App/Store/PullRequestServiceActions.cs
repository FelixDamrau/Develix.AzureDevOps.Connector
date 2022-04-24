using Develix.AzureDevOps.Connector.App.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record LoginPullRequestServiceAction(Uri AzureDevopsOrgUri, string Token) : ILoginServiceAction;

public record LoginPullRequestServiceResultAction(ConnectionStatus ConnectionStatus);
