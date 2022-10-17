using System.Text.RegularExpressions;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Develix.AzureDevOps.Connector.Service.Logic;

internal static class PullRequestFactory
{
    public static bool IsPullRequestRelation(WorkItemRelation workItemRelation)
    {
        return string.Equals(workItemRelation.Rel, "ArtifactLink", StringComparison.OrdinalIgnoreCase)
            && workItemRelation.Attributes.TryGetValue("name", out var value)
            && value is string stringValue
            && string.Equals(stringValue, "Pull Request", StringComparison.OrdinalIgnoreCase);
    }

    public static Result<int> GetPullRequestId(WorkItemRelation workItemRelation)
    {
        if (workItemRelation.Url is string url
            && Regex.Split(url, "%2F", RegexOptions.IgnoreCase) is { } splittedUrl
            && splittedUrl.Length == 3
            && int.TryParse(splittedUrl[2], out var id))
        {
            return Result.Ok(id);
        }
        return Result.Fail<int>($"The pull request id could not be extracted");
    }

    public static Model.PullRequest Create(GitPullRequest pullRequest)
    {
        return new Model.PullRequest(
            pullRequest.PullRequestId,
            pullRequest.Status.ToModel(),
            pullRequest.Repository.Name,
            new(pullRequest.Repository.WebUrl, UriKind.Absolute),
            pullRequest.Title,
            pullRequest.CreatedBy.DisplayName,
            pullRequest.SourceRefName[11..],
            pullRequest.TargetRefName[11..]);
    }

    private static Model.PullRequestStatus ToModel(this PullRequestStatus status)
    {
        return status switch
        {
            PullRequestStatus.Active => Model.PullRequestStatus.Active,
            PullRequestStatus.Abandoned => Model.PullRequestStatus.Abandoned,
            PullRequestStatus.Completed => Model.PullRequestStatus.Completed,
            _ => Model.PullRequestStatus.Invalid
        };
    }
}
