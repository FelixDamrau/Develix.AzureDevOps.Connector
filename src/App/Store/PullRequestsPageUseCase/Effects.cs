using Develix.AzureDevOps.Connector.Service;
using Fluxor;
using MudBlazor;

namespace Develix.AzureDevOps.Connector.App.Store.PullRequestsPageUseCase;

public class Effects
{
    private readonly IReposService reposService;
    private readonly ISnackbar snackbar;

    public Effects(IReposService reposService, ISnackbar snackbar)
    {
        this.reposService = reposService;
        this.snackbar = snackbar;
    }

    [EffectMethod]
    public async Task HandleGetPullRequestsAction(GetPullRequestsAction action, IDispatcher dispatcher)
    {
        var pullRequests = reposService.GetPullRequests(action.Ids);
        var results = await pullRequests.ToListAsync().ConfigureAwait(false);
        var resultAction = new GetPullRequestsResultAction(results.Where(r => r.Valid).Select(r => r.Value).ToList());
        foreach (var message in results.Where(r => !r.Valid).Select(r => r.Message))
        {
            snackbar.Add(message, Severity.Warning);
        }
        dispatcher.Dispatch(resultAction);
    }
}
