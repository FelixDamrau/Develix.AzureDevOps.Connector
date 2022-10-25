using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal class CreateIterationRequest : WorkItemTrackingRequestBase<string>
{
    private readonly string project;
    private readonly string name;
    private readonly DateTime startDate;
    private readonly DateTime finishDate;

    public CreateIterationRequest(WorkItemTrackingLogin? login, string project, string name, DateTime startDate, DateTime finishDate)
        : base(login)
    {
        this.project = project ?? throw new ArgumentNullException(nameof(project));
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.startDate = startDate;
        this.finishDate = finishDate;
    }

    protected override async Task<string> Execute(WorkItemTrackingLogin login, CancellationToken cancellationToken = default)
    {
        var node = new WorkItemClassificationNode()
        {
            Name = name,
            Attributes = new Dictionary<string, object>
            {
                { "startDate", startDate },
                { "finishDate", finishDate },
            }
        };
        var createdNode = await login.VssClient
            .CreateOrUpdateClassificationNodeAsync(
                node,
                project,
                TreeStructureGroup.Iterations,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return createdNode.Name;
    }
}
