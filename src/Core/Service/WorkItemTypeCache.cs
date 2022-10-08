using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemTypeCache
{
    private readonly WorkItemTrackingHttpClient workItemTrackingHttpClient;
    private readonly Dictionary<string, ProjectWorkItemCache> cache;

    public WorkItemTypeCache(WorkItemTrackingHttpClient workItemTrackingHttpClient)
    {
        this.workItemTrackingHttpClient = workItemTrackingHttpClient;
        cache = new();
    }

    public async Task<WorkItemType> GetWorkItemType(string project, string workItemTypeName)
    {
        var typeCache = await GetProjectWorkItemCache(project);
        if (typeCache.TryGetValue(workItemTypeName, out var workItemType))
            return workItemType;
        return WorkItemType.Unknown;
    }

    private async Task<ProjectWorkItemCache> GetProjectWorkItemCache(string project)
    {
        if (!cache.ContainsKey(project))
            cache[project] = await CreateProjectWorkItemCache(project);

        return cache[project];
    }

    private async Task<ProjectWorkItemCache> CreateProjectWorkItemCache(string project)
    {
        var workItemTypes = await workItemTrackingHttpClient.GetWorkItemTypesAsync(project);
        var workItemTypesDictionary = workItemTypes
            .Select(tfWi => new WorkItemType(tfWi.Name, tfWi.Description, tfWi.Color, tfWi.Icon.Url))
            .ToDictionary(wi => wi.Name);
        return new(project, workItemTypesDictionary);
    }

    private class ProjectWorkItemCache
    {
        private IReadOnlyDictionary<string, WorkItemType> cache;
        public string Project { get; }

        public ProjectWorkItemCache(string project, IReadOnlyDictionary<string, WorkItemType> cache)
        {
            Project = project;
            this.cache = cache;
        }

        public bool TryGetValue(string typeName, [NotNullWhen(true)] out WorkItemType? value) => cache.TryGetValue(typeName, out value);
    }
}
