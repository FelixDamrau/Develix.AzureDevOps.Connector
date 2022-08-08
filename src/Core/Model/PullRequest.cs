namespace Develix.AzureDevOps.Connector.Model;

public record PullRequest
{
    public int Id { get; init; } = -1;

    public PullRequestStatus Status { get; init; } = PullRequestStatus.Invalid;

    public string Author { get; init; } = "Unknwon";

    public string Title { get; init; } = "Unknown";
}
