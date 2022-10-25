using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal class GetAreaPathsRequest : WorkItemTrackingRequestBase<IReadOnlyList<AreaPath>>
{
    private readonly string project;
    private readonly int depth;

    public GetAreaPathsRequest(WorkItemTrackingLogin? login, string project, int depth)
        : base(login)
    {
        this.project = project ?? throw new ArgumentNullException(nameof(project));
        this.depth = depth;
    }

    protected override async Task<IReadOnlyList<AreaPath>> Execute(WorkItemTrackingLogin login, CancellationToken cancellationToken = default)
    {
        var areaPaths = await login.VssClient
            .GetClassificationNodeAsync(project, TreeStructureGroup.Areas, depth: depth, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        Func<WorkItemClassificationNode, IEnumerable<WorkItemClassificationNode>>? flatten = null;
        flatten = n => new[] { n }
            .Concat(n.Children is null
                ? Enumerable.Empty<WorkItemClassificationNode>()
                : n.Children.SelectMany(c => flatten!(c)));

        return flatten(areaPaths).Select(p => new AreaPath { Id = p.Id, Name = p.Path }).ToList();
    }
}
