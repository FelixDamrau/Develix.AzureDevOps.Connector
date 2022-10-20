using Develix.AzureDevOps.Connector.Model;

namespace Develix.AzureDevOps.Connector.App.Store;

public record CreateWorkItemAction(string WorkItemType);
public record CreateWorkItemResultAction();
public record GetWorkItemTypesAction(string Project);
public record GetWorkItemTypesResultAction(IReadOnlyList<WorkItemType> WorkItemTypes);
