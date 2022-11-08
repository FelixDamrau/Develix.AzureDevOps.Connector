using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class ReposService : VssService<GitHttpClient, GitClientLogin>, IReposService
{
    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<Model.PullRequest>>> GetPullRequests(IEnumerable<int> ids)
    {
        if (!IsInitialized())
            return Result.Fail<IReadOnlyList<Model.PullRequest>>("Service is not initialized.");

        var requests = ids.Select(id => GetPullRequest(azureDevopsLogin.VssClient, id));
        var results = await Task.WhenAll(requests).ConfigureAwait(false);
        IReadOnlyList<Model.PullRequest> pullRequests = results.Where(x => x.Valid).Select(x => x.Value).ToList();
        return Result.Ok(pullRequests);
    }

    protected override GitClientLogin CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return GitClientLogin.Create(baseUri, azureDevopsWorkItemReadToken);
    }

    private static async Task<Result<Model.PullRequest>> GetPullRequest(GitHttpClient prClient, int id)
    {
        try
        {
            var pr = await prClient.GetPullRequestByIdAsync(id).ConfigureAwait(false);
            return Result.Ok(PullRequestFactory.Create(pr));
        }
        catch (Exception ex)
        {
            return Result.Fail<Model.PullRequest>($"Could not create pull request with id {id} - Message: {ex.Message}");
        }
    }

    protected override async Task CheckConnection(GitHttpClient vssClient)
    {
        await vssClient.GetRepositoriesAsync().ConfigureAwait(false);
    }
}
