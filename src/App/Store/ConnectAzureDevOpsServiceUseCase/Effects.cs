using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.ConnectAzureDevOpsServiceUseCase;

public class Effects
{
    public Effects(IReposService reposService, IWorkItemService workItemService)
    {
        ReposService = reposService ?? throw new ArgumentNullException(nameof(reposService));
        WorkItemService = workItemService ?? throw new ArgumentNullException(nameof(workItemService));
    }

    public IReposService ReposService { get; set; }
    public IWorkItemService WorkItemService { get; set; }

    [EffectMethod]
    public async Task HandleLoginRepoServiceAction(LoginRepoServiceAction action, IDispatcher dispatcher)
    {
        var login = await ReposService.Initialize(action.AzureDevopsOrgUri, action.Token);
        var resultAction = new LoginRepoServiceResultAction(login.Valid ? Model.ConnectionStatus.Connected : Model.ConnectionStatus.NotConnected);
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
