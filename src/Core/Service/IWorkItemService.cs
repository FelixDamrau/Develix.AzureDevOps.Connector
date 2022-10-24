using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public interface IWorkItemService : IAzureDevOpsService
{
    Task<Result<IReadOnlyList<WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests);

    Task<Result<int>> CreateWorkItem(WorkItemCreateTemplate template);

    Task<Result<IReadOnlyList<WorkItemType>>> GetWorkItemTypes(string project);

    Task<Result<IReadOnlyList<AreaPath>>> GetAreaPaths(string project, int depth);

    Task<Result<string>> CreateIteration(string project, string name, DateTime startDate, DateTime finishDate);
}
