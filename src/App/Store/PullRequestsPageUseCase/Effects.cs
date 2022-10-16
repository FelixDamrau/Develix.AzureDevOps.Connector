using Develix.AzureDevOps.Connector.App.Services;
using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PullRequestsPageUseCase;

public class Effects
{
    private readonly IReposService reposService;
    private readonly ISnackbarService snackbarService;

    public Effects(IReposService reposService, ISnackbarService snackbarService)
    {
        this.reposService = reposService;
        this.snackbarService = snackbarService;
    }

    [EffectMethod]
    public async Task HandleGetPullRequestsAction(GetPullRequestsAction action, IDispatcher dispatcher)
    {
        var pullRequestsResult = await reposService.GetPullRequests(action.Ids).ConfigureAwait(false);
        var pullRequests = pullRequestsResult.Valid ? pullRequestsResult.Value : Array.Empty<PullRequest>();
        dispatcher.Dispatch(new GetPullRequestsResultAction(pullRequests));

        if (pullRequestsResult.Valid)
        {
            var notFoundIds = action.Ids.Except(pullRequests.Select(pr => pr.Id));
            NotifyNotFoundIds(notFoundIds);
        }
        else
        {
            snackbarService.SendError($"Could not get pull requests. Message: {pullRequestsResult.Message}");
        }
    }

    private void NotifyNotFoundIds(IEnumerable<int> notFoundIds)
    {
        if (notFoundIds.Any())
        {
            var message = $"Could not find pull requests with IDs: {string.Join(", ", notFoundIds)}";
            snackbarService.SendWarning(message);
        }
    }
}
