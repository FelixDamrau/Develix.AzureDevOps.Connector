using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public interface IWorkItemService : IAzureDevOpsService
{
    Task<Result<IReadOnlyList<WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests);

    Task<Result<Model.WorkItem>> CreateWorkItem(Model.WorkItemCreateTemplate template);

    Task<Result<IReadOnlyList<Model.WorkItemType>>> GetWorkItemTypes(string project);

    Task<Result<IReadOnlyList<string>>> GetAreaPaths(string project, int depth);
}
