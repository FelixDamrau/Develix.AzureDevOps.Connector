namespace Develix.AzureDevOps.Connector.Service.Logic;

internal record WorkItemTypeCacheKey(string Project) : ICacheKey;

internal record WorkItemStatusCacheKey(string Project, string WorkItemType) : ICacheKey;

internal interface ICacheKey
{
    string Project { get; }
}
