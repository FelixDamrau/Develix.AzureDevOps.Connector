using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public interface IWorkItemService : IAzureDevOpsService
{
    Task<Result<IReadOnlyList<WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests);
}
