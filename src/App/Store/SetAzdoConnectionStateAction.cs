namespace Develix.AzureDevOps.Connector.App.Store;

public record SetAzdoConnectionStateAction(Uri AzureDevopsOrgUri, string Token);
