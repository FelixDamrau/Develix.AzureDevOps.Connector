using Develix.AzureDevOps.Connector.App.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record LoginWorkItemServiceAction(Uri AzureDevopsOrgUri, string Token) : ILoginServiceAction;

public record LoginWorkItemServiceResultAction(ConnectionStatus ConnectionStatus);
