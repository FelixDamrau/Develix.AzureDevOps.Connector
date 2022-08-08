using System.Diagnostics.CodeAnalysis;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class ReposService : IReposService, IDisposable
{
    private Uri? azureDevopsOrgUri;
    private string? azureDevopsPullRequestReadToken;
    private ServiceState state = ServiceState.NotInitialized;
    private GitHttpClient? gitHttpClient;
    private bool disposedValue;

    /// <inheritdoc/>
    public async Task<Result> Initialize(Uri azureDevopsOrgUri, string azureDevopsPullRequestReadToken)
    {
        this.azureDevopsOrgUri = azureDevopsOrgUri;
        this.azureDevopsPullRequestReadToken = azureDevopsPullRequestReadToken;
        var serviceState = await Login();
        return serviceState.Valid ? Result.Ok() : Result.Fail(serviceState.Message);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Model.PullRequest> GetPullRequests(IEnumerable<int> ids)
    {
        if (IsInitialized())
        {
            foreach (var id in ids)
            {
                yield return await GetPullRequest(gitHttpClient, id);
            }
        }
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(gitHttpClient))]
    public bool IsInitialized() => state == ServiceState.Initialized && gitHttpClient is not null;

    private async Task<Result> Login()
    {
        try
        {
            var credential = new VssBasicCredential(string.Empty, azureDevopsPullRequestReadToken);
            gitHttpClient = new GitHttpClient(azureDevopsOrgUri, credential);
            _ = await gitHttpClient.GetRepositoriesAsync(); // Perform a simple call to check if the connection is valid
            state = ServiceState.Initialized;
            return Result.Ok();
        }
        catch (VssUnauthorizedException e)
        {
            return Error("Authorization failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (VssServiceResponseException e)
        {
            return Error("Connection failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (Exception e)
        {
            return Error("Unknown error!" + Environment.NewLine + "Error message: " + e.Message);
        }

        Result Error(string message)
        {
            state = ServiceState.InitializationFailed;
            return Result.Fail(message);
        }
    }

    private static async Task<Model.PullRequest> GetPullRequest(GitHttpClient prClient, int id)
    {
        try
        {
            var pr = await prClient.GetPullRequestByIdAsync(id);
            return PullRequestFactory.Create(pr);
        }
        catch (Exception)
        {
            return PullRequestFactory.GetDefaultInvalid() with { Id = id, Status = Model.PullRequestStatus.Invalid };
        }
    }

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                gitHttpClient?.Dispose();
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
