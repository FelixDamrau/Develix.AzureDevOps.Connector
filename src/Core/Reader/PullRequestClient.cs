using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Develix.AzureDevOps.Connector.Reader;

public class PullRequestClient
{
    private readonly Uri azureDevopsOrgUri;
    private readonly string azureDevopsPullRequestReadToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="PullRequestClient" /> class.
    /// </summary>
    /// <param name="azureDevopsOrgUri">The URI of the azure devops organization, e.g.: https://dev.azure.com/myOrgName </param>
    /// <param name="azureDevopsPullRequestReadToken">A from the provided <paramref name="orgName"/> with at least a pull request read permission.</param>
    public PullRequestClient(Uri azureDevopsOrgUri, string azureDevopsPullRequestReadToken)
    {
        this.azureDevopsOrgUri = azureDevopsOrgUri;
        this.azureDevopsPullRequestReadToken = azureDevopsPullRequestReadToken;
    }

    public async IAsyncEnumerable<Model.PullRequest> GetPullRequests(IEnumerable<int> ids)
    {
        var credentials = new VssBasicCredential(string.Empty, azureDevopsPullRequestReadToken);
        using var prClient = new GitHttpClient(azureDevopsOrgUri, credentials);
        foreach (var id in ids)
        {
            yield return await GetPullRequest(prClient, id);
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
}
