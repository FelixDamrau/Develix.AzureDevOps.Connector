using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record CreateWorkItemAction(WorkItemCreateTemplate WorkItemTemplate);
public record CreateWorkItemResultAction(WorkItem WorkItem);
public record GetWorkItemTypesAction(string Project);
public record GetWorkItemTypesResultAction(IReadOnlyList<WorkItemType> WorkItemTypes);
