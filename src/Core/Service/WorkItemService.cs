using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.AzureDevOps.Connector.Service.Requests;
using Develix.Essentials.Core;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public class WorkItemService : VssService<WorkItemTrackingHttpClient, WorkItemTrackingLogin>, IWorkItemService
{
    private readonly IReposService reposService;

    public WorkItemService(IReposService reposService)
    {
        this.reposService = reposService;
    }

    public async Task<Result<IReadOnlyList<Model.WorkItem>>> GetWorkItems(IEnumerable<int> ids, bool includePullRequests)
    {
        var request = new GetWorkItemsRequest(azureDevopsLogin, reposService, ids, includePullRequests);
        return await request.Execute().ConfigureAwait(false);
    }

    public async Task<Result<int>> CreateWorkItem(Model.WorkItemCreateTemplate template)
    {
        var request = new CreateWorkItemRequest(azureDevopsLogin, template);
        return await request.Execute().ConfigureAwait(false);
    }

    public async Task<Result<IReadOnlyList<Model.WorkItemType>>> GetWorkItemTypes(string project)
    {
        var request = new GetWorkItemTypesRequest(azureDevopsLogin, project);
        return await request.Execute().ConfigureAwait(false);

    }

    public async Task<Result<IReadOnlyList<Model.AreaPath>>> GetAreaPaths(string project, int depth)
    {
        var request = new GetAreaPathsRequest(azureDevopsLogin, project, depth);
        return await request.Execute().ConfigureAwait(false);
    }

    public async Task<Result<string>> CreateIteration(string project, string name, DateTime startDate, DateTime finishDate)
    {
        var request = new CreateIterationRequest(azureDevopsLogin, project, name, startDate, finishDate);
        return await request.Execute().ConfigureAwait(false);
    }

    protected override async Task<WorkItemTrackingLogin> CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken)
    {
        return await WorkItemTrackingLogin.Create(baseUri, azureDevopsWorkItemReadToken).ConfigureAwait(false);
    }
}
