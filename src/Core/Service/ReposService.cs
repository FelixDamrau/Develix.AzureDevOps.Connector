using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class ReposService : VssService<GitHttpClient, GitClientLogin>, IReposService, IDisposable
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

    #region IDisposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                azureDevopsLogin?.VssClient?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
