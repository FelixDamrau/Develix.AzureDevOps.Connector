using Develix.AzureDevOps.Connector.App.Services;
using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PackagesPageUseCase;
public class Effects
{
    private readonly IPackagesService packagesService;
    private readonly ISnackbarService snackbarService;

    public Effects(IPackagesService packagesService, ISnackbarService snackbarService)
    {
        this.packagesService = packagesService;
        this.snackbarService = snackbarService;
    }

    [EffectMethod]
    public async Task HandleGetAllPackagesAction(GetAllPackagesAction action, IDispatcher dispatcher)
    {
        var packagesResult = await packagesService.GetPackages(action.Project, action.Feed).ConfigureAwait(false);
        var packages = packagesResult.Valid ? packagesResult.Value : Array.Empty<Package>();

        var resultAction = new GetPackagesResultAction(packages);
        dispatcher.Dispatch(resultAction);
        if (!packagesResult.Valid)
            snackbarService.SendError("Could not get packages!", packagesResult.Message);
    }

    [EffectMethod]
    public async Task HandleGetPackageAction(GetPackageAction action, IDispatcher dispatcher)
    {
        var packageResult = await packagesService.GetPackage(action.Project, action.Feed, action.PackageName).ConfigureAwait(false);
        var packages = packageResult.Valid ? new[] { packageResult.Value } : Array.Empty<Package>();
        var resultAction = new GetPackagesResultAction(packages);
        dispatcher.Dispatch(resultAction);
        if (!packageResult.Valid)
            snackbarService.SendError("Could not get package!", packageResult.Message);
    }
}
