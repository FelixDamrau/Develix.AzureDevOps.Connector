using Develix.AzureDevOps.Connector.App.Model;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store;

[FeatureState]
public record AzureDevOpsServicesState
{
    public ConnectionStatus PullRequestServiceConnectionStatus { get; init; } = ConnectionStatus.NotConnected;
    public ConnectionStatus WorkItemServiceConnectionStatus { get; init; } = ConnectionStatus.NotConnected;
}
