using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Develix.AzureDevOps.Connector.Service;
internal class ProjectService : VssService<ProjectHttpClient, ProjectServiceLogin>
{
    protected override Task<ProjectServiceLogin> CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return Task.FromResult(ProjectServiceLogin.Create(baseUri, azureDevopsWorkItemReadToken));
    }
}
