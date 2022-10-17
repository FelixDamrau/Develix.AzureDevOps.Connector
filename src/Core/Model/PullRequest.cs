namespace Develix.AzureDevOps.Connector.Model;

public record PullRequest(
    int Id,
    PullRequestStatus Status,
    string RepositoryName,
    Uri RespositorySource,
    string Title,
    string Author,
    string TargetBranch,
    string SourceBranch);
