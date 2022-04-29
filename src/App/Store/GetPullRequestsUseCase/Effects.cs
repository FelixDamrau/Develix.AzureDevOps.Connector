using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.GetPullRequestsUseCase;

public class Effects
{
    private readonly IPullRequestService pullRequestService;

    public Effects(IPullRequestService pullRequestService)
    {
        this.pullRequestService = pullRequestService;
    }

    [EffectMethod]
    public async Task HandleGetPullRequestsAction(GetPullRequestsAction action, IDispatcher dispatcher)
    {
        var pullRequests = pullRequestService.GetPullRequests(action.Ids);
        var resultAction = new GetPullRequestsResultAction(await pullRequests.ToListAsync());
        dispatcher.Dispatch(resultAction);
    }
}
