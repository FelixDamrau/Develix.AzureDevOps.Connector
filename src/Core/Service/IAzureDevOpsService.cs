using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public interface IAzureDevOpsService
{
    /// <summary>
    /// Initializes this service and performs a connection check to indicate whether the connection is established successfully.
    /// </summary>
    /// <param name="azureDevopsOrgUri">The URI of the azure devops organization (e.g.: https://dev.azure.com/myOrgName/).</param>
    /// <param name="token">A token the provided <paramref name="orgName"/>.</param>
    Task<Result> CheckedInitialize(Uri azureDevopsOrgUri, string token);

    /// <summary>
    /// Initializes this service.
    /// </summary>
    /// <param name="azureDevopsOrgUri">The URI of the azure devops organization (e.g.: https://dev.azure.com/myOrgName/).</param>
    /// <param name="token">A token the provided <paramref name="orgName"/>.</param>
    void Initialize(Uri azureDevopsOrgUri, string token);

    /// <summary>
    /// Checks if the service is successfully initialized.
    /// </summary>
    bool IsInitialized();
}
