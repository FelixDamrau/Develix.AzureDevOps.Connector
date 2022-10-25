using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Develix.AzureDevOps.Connector.Service.Logic;
internal class ProjectServiceLogin : AzureDevopsLogin<ProjectHttpClient>
{
    public ProjectServiceLogin(Uri azureDevopsOrgUri, ProjectHttpClient vssClient)
        : base(azureDevopsOrgUri, vssClient)
    {
    }

    public static ProjectServiceLogin Create(Uri azureDevopsOrgUri, string token)
    {
        var credential = new VssBasicCredential(string.Empty, token);
        var vssClient = new ProjectHttpClient(azureDevopsOrgUri, credential);
        //await ConnectionCheck(vssClient).ConfigureAwait(false);
        return new ProjectServiceLogin(azureDevopsOrgUri, vssClient);
    }
}
