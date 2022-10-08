namespace Develix.AzureDevOps.Connector.Service.Logic;

internal record WorkItemTypeCacheKey(string Project);

internal record WorkItemStatusCacheKey(string Project, string WorkItemType);
