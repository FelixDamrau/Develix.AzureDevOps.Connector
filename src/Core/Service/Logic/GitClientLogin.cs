using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Develix.AzureDevOps.Connector.Service.Logic;
public class GitClientLogin : AzureDevopsLogin<GitHttpClient>
{
    private GitClientLogin(Uri azureDevopsOrgUri, GitHttpClient vssClient)
        : base(azureDevopsOrgUri, vssClient)
    {
    }

    public static async Task<GitClientLogin> Create(Uri azureDevopsOrgUri, string token)
    {
        var credential = new VssBasicCredential(string.Empty, token);
        var vssClient = new GitHttpClient(azureDevopsOrgUri, credential);
        await ConnectionCheck(vssClient).ConfigureAwait(false);
        return new GitClientLogin(azureDevopsOrgUri, vssClient);
    }

    private static async Task ConnectionCheck(GitHttpClient client)
    {
        await client.GetRepositoriesAsync().ConfigureAwait(false);
    }
}
