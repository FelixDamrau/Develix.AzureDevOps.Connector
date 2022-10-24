using Develix.AzureDevOps.Connector.Model;
using Develix.AzureDevOps.Connector.Service.Logic;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal class CreateWorkItemRequest : WorkItemTrackingRequestBase<WorkItem>
{
    private readonly WorkItemCreateTemplate template;

    public CreateWorkItemRequest(WorkItemTrackingLogin? login, WorkItemCreateTemplate template)
        : base(login)
    {
        this.template = template;
    }

    protected override async Task<WorkItem> Execute(WorkItemTrackingLogin login, CancellationToken cancellationToken = default)
    {
        var azdoWorkItem = await login.VssClient
            .CreateWorkItemAsync(
                WorkItemTemplateFactory.Create(template, login.AzureDevopsOrgUri.AbsoluteUri),
                template.Project,
                template.WorkItemType,
                validateOnly: false,
                bypassRules: true,
                userState: false,
                CancellationToken.None)
            .ConfigureAwait(false);

        return await login.WorkItemFactory.Create(azdoWorkItem, login.AzureDevopsOrgUri).ConfigureAwait(false);
    }
}
