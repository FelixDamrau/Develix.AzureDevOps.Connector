using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class ReposService : VssService<GitHttpClient, GitClientLogin>, IReposService
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<Model.PullRequest> GetPullRequests(IEnumerable<int> ids)
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

    private static async Task<Model.PullRequest> GetPullRequest(GitHttpClient prClient, int id)
    {
        try
        {
            var pr = await prClient.GetPullRequestByIdAsync(id).ConfigureAwait(false);
            return PullRequestFactory.Create(pr);
        }
        catch (Exception)
        {
            return PullRequestFactory.GetDefaultInvalid() with { Id = id, Status = Model.PullRequestStatus.Invalid };
        }
    }
}
