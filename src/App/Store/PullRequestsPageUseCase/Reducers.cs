using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PullRequestsPageUseCase;

public static class Reducers
{
    [ReducerMethod(typeof(GetPullRequestsAction))]
    public static PullRequestPageState ReduceGetPullRequestsAction(PullRequestPageState state)
    {
        return state with { IsLoading = true };
    }

    [ReducerMethod]
    public static PullRequestPageState ReduceGetPullRequestsResultAction(PullRequestPageState state, GetPullRequestsResultAction action)
    {
        return state with { IsLoading = false, PullRequests = action.PullRequests };
    }
}
