using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class ReposService : VssService<GitHttpClient, GitClientLogin>, IReposService
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<Result<Model.PullRequest>> GetPullRequests(IEnumerable<int> ids)
    {
        IsInitializedGuard();

        foreach (var id in ids)
        {
            yield return await GetPullRequest(azureDevopsLogin.VssClient, id).ConfigureAwait(false);
        }
    }

    protected override async Task<GitClientLogin> CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return await GitClientLogin.Create(baseUri, azureDevopsWorkItemReadToken).ConfigureAwait(false);
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
}
