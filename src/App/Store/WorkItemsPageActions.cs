using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record GetWorkItemsAction(IEnumerable<int> Ids);

public record GetWorkItemsResultAction(IReadOnlyList<WorkItem> WorkItems);
