using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PackagesPageUseCase;

public static class Reducers
{
    [ReducerMethod(typeof(GetAllPackagesAction))]
    public static PackagesPageState ReduceGetAllPackagesAction(PackagesPageState state)
    {
        return state with
        {
            IsLoading = true,
        };
    }

    [ReducerMethod]
    public static PackagesPageState ReduceGetAllPackagesResultAction(PackagesPageState state, GetPackagesResultAction action)
    {
        return state with
        {
            IsLoading = false,
            Packages = action.Packages,
        };
    }

    [ReducerMethod(typeof(GetPackageAction))]
    public static PackagesPageState ReduceGetPackageAction(PackagesPageState state)
    {
        return state with
        {
            IsLoading = true,
        };
    }
}
