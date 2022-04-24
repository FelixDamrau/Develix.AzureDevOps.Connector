using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;
public interface IAzureDevOpsService
{
    /// <summary>
    /// Initializes this service.
    /// </summary>
    /// <param name="azureDevopsOrgUri">The URI of the azure devops organization, e.g.: https://dev.azure.com/myOrgName </param>
    /// <param name="azureDevopsPullRequestReadToken">A token the provided <paramref name="orgName"/> with at least a pull request read permission.</param>
    Task<Result> Initialize(Uri azureDevopsOrgUri, string azureDevopsPullRequestReadToken);

    /// <summary>
    /// Checks if the service is successfully initialized.
    /// </summary>
    bool IsInitialized();
}
