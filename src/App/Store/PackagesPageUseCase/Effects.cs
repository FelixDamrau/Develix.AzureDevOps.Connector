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
        var packagesResult = await packagesService.GetPackages(action.Project, action.Feed);
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
}
