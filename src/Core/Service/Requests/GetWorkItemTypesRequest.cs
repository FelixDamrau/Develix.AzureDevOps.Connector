using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service.Logic;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal class GetWorkItemTypesRequest : WorkItemTrackingRequestBase<IReadOnlyList<WorkItemType>>
{
    private readonly string project;

    public GetWorkItemTypesRequest(WorkItemTrackingLogin? login, string project)
        : base(login)
    {
        this.project = project ?? throw new ArgumentNullException(nameof(project));
    }

    protected override async Task<IReadOnlyList<WorkItemType>> Execute(WorkItemTrackingLogin login, CancellationToken cancellationToken = default)
    {
        var azdoWorkItemTypes = await login.VssClient
            .GetWorkItemTypesAsync(project, cancellationToken: cancellationToken).ConfigureAwait(false);
        return azdoWorkItemTypes
            .Select(t => new Model.WorkItemType(t.Name, t.Description, t.Color, t.Icon.Url))
            .ToList();
    }
}
