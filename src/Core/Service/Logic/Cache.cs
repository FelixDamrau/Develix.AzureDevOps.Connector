using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal abstract class Cache<T, TKey>(WorkItemTrackingHttpClient workItemTrackingHttpClient, T fallback)
    where T : class
    where TKey : IEquatable<TKey>
{
    protected readonly WorkItemTrackingHttpClient workItemTrackingHttpClient = workItemTrackingHttpClient;
    private readonly T fallback = fallback;
    private readonly ConcurrentDictionary<TKey, ItemCache> cache = [];

    public async Task<T> Get(TKey cacheReference, string key)
    {
        var itemCache = await GetItemCache(cacheReference).ConfigureAwait(false);
        if (itemCache.TryGetValue(key, out var value))
            return value;
        return fallback;
    }

    private async Task<ItemCache> GetItemCache(TKey key)
    {
        if (!cache.ContainsKey(key))
            cache[key] = await CreateItemCache(key).ConfigureAwait(false);

        return cache[key];
    }

    protected abstract Task<ItemCache> CreateItemCache(TKey key);

    protected class ItemCache
    {
        private readonly IReadOnlyDictionary<string, T> cache;

        public ItemCache(IReadOnlyDictionary<string, T> cache)
        {
            this.cache = cache;
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out T? value) => cache.TryGetValue(key, out value);
    }
}
