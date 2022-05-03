using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PullRequestsPageUseCase;

public class Effects
{
    private readonly IReposService reposService;

    public Effects(IReposService reposService)
    {
        this.reposService = reposService;
    }

    [EffectMethod]
    public async Task HandleGetPullRequestsAction(GetPullRequestsAction action, IDispatcher dispatcher)
    {
        var pullRequests = reposService.GetPullRequests(action.Ids);
        var resultAction = new GetPullRequestsResultAction(await pullRequests.ToListAsync());
        dispatcher.Dispatch(resultAction);
    }
}
