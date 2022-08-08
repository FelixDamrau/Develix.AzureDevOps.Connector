using Develix.AzureDevOps.Connector.Model;
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

    [ReducerMethod(typeof(GetPackageAction))]
    public static PackagesPageState ReduceGetPackageAction(PackagesPageState state)
    {
        return state with
        {
            ErrorMessage = default,
            IsLoading = true,
        };
    }

    [ReducerMethod]
    public static PackagesPageState ReduceGetPackageResultAction(PackagesPageState state, GetPackageResultAction action)
    {
        return state with
        {
            ErrorMessage = action.ErrorMessage,
            IsLoading = false,
            Packages = action.Package is not null ? new[] { action.Package } : Array.Empty<Package>(),
        };
    }
}
