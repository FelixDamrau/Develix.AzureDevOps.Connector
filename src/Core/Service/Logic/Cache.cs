using System.Diagnostics.CodeAnalysis;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal abstract class Cache<T>
    where T : class
{
    protected readonly WorkItemTrackingHttpClient workItemTrackingHttpClient;
    private readonly T fallback;
    private readonly Dictionary<string, ProjectItemCache> cache;

    protected Cache(WorkItemTrackingHttpClient workItemTrackingHttpClient, T fallback)
    {
        this.workItemTrackingHttpClient = workItemTrackingHttpClient;
        this.fallback = fallback;
        cache = new();
    }

    public async Task<T> Get(string project, string key)
    {
        var itemCache = await GetItemCache(project);
        if (itemCache.TryGetValue(key, out var value))
            return value;
        return fallback;
    }

    private async Task<ProjectItemCache> GetItemCache(string project)
    {
        if (!cache.ContainsKey(project))
            cache[project] = await CreateItemCache(project);

        return cache[project];
    }

    protected abstract Task<ProjectItemCache> CreateItemCache(string project);

    protected class ProjectItemCache
    {
        private IReadOnlyDictionary<string, T> cache;
        public string Project { get; }

        public ProjectItemCache(string project, IReadOnlyDictionary<string, T> cache)
        {
            Project = project;
            this.cache = cache;
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out T? value) => cache.TryGetValue(key, out value);
    }
}
