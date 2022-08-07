using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PackagesPageUseCase;

public static class Reducers
{
    [ReducerMethod(typeof(GetAllPackagesAction))]
    public static PackagesPageState ReduceGetAllPackagesAction(PackagesPageState state)
    {
        return state with
        {
            ErrorMessage = default,
            IsLoading = true,
        };
    }

    [ReducerMethod]
    public static PackagesPageState ReduceGetAllPackagesResultAction(PackagesPageState state, GetAllPackagesResultAction action)
    {
        return state with
        {
            ErrorMessage = action.ErrorMessage,
            IsLoading = false,
            Packages = action.Packages,
        };
    }
}
