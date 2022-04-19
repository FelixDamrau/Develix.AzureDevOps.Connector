namespace Develix.AzureDevOps.Connector.Model;

public class WorkItem
{
    public int Id { get; init; }

    public string Title { get; init; } = "No title";

    public string TeamProject { get; init; } = "Unknown";

    public WorkItemStatus Status { get; init; } = WorkItemStatus.Invalid;

    public WorkItemType WorkItemType { get; init; } = WorkItemType.Invalid;

    private readonly List<PullRequest> pullRequests = new();
    public IReadOnlyList<PullRequest> PullRequests => pullRequests;

    public string? AzureDevopsLink { get; init; }

    public void AddPullRequest(PullRequest pullRequest) => pullRequests.Add(pullRequest);

}
