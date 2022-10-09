using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.PackagesPageUseCase;
public class Effects
{
    private readonly IPackagesService packagesService;

    public Effects(IPackagesService packagesService)
    {
        this.packagesService = packagesService;
    }

    [EffectMethod]
    public async Task HandleGetAllPackagesAction(GetAllPackagesAction action, IDispatcher dispatcher)
    {
        var packagesResult = await packagesService.GetPackages(action.Project, action.Feed).ConfigureAwait(false);
        if (packagesResult.Valid)
        {
            var resultAction = new GetAllPackagesResultAction(packagesResult.Value);
            dispatcher.Dispatch(resultAction);
        }
        else
        {
            var resultAction = new GetAllPackagesResultAction(Array.Empty<Package>(), packagesResult.Message); // TODO Error handling!
            dispatcher.Dispatch(resultAction);
        }
    }

    [EffectMethod]
    public async Task HandleGetPackageAction(GetPackageAction action, IDispatcher dispatcher)
    {
        var packageResult = await packagesService.GetPackage(action.Project, action.Feed, action.PackageName).ConfigureAwait(false);
        if (packageResult.Valid)
        {
            var resultAction = new GetPackageResultAction(packageResult.Value);
            dispatcher.Dispatch(resultAction);
        }
        else
        {
            var resultAction = new GetPackageResultAction(null, packageResult.Message); // TODO Error handling!
            dispatcher.Dispatch(resultAction);
        }
    }
}
