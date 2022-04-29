using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.ConnectAzureDevOpsServiceUseCase;

public class Effects
{
    public Effects(IPullRequestService pullRequestService, IWorkItemService workItemService)
    {
        PullRequestService = pullRequestService ?? throw new ArgumentNullException(nameof(pullRequestService));
        WorkItemService = workItemService ?? throw new ArgumentNullException(nameof(workItemService));
    }

    public IPullRequestService PullRequestService { get; set; }
    public IWorkItemService WorkItemService { get; set; }

    [EffectMethod]
    public async Task HandleLoginPullRequestServiceAction(LoginPullRequestServiceAction action, IDispatcher dispatcher)
    {
        var login = await PullRequestService.Initialize(action.AzureDevopsOrgUri, action.Token);
        var resultAction = new LoginPullRequestServiceResultAction(login.Valid ? Model.ConnectionStatus.Connected : Model.ConnectionStatus.NotConnected);
        dispatcher.Dispatch(resultAction);
    }

    [EffectMethod]
    public async Task HandleLoginWorkItemServiceAction(LoginWorkItemServiceAction action, IDispatcher dispatcher)
    {
        var login = await WorkItemService.Initialize(action.AzureDevopsOrgUri, action.Token);
        var resultAction = new LoginWorkItemServiceResultAction(login.Valid ? Model.ConnectionStatus.Connected : Model.ConnectionStatus.NotConnected);
        dispatcher.Dispatch(resultAction);
    }
}
