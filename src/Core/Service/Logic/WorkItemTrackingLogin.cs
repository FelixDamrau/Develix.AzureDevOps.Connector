using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Develix.AzureDevOps.Connector.Service.Logic;

public class WorkItemTrackingLogin : AzureDevopsLogin<WorkItemTrackingHttpClient>
{
    private WorkItemTrackingLogin(Uri azureDevopsOrgUri, WorkItemTrackingHttpClient vssClient, WorkItemFactory workItemFactory)
        : base(azureDevopsOrgUri, vssClient)
    {
        WorkItemFactory = workItemFactory;
    }

    public WorkItemFactory WorkItemFactory { get; }

    public static WorkItemTrackingLogin Create(Uri azureDevopsOrgUri, string token)
    {
        var credential = new VssBasicCredential(string.Empty, token);
        var vssClient = new WorkItemTrackingHttpClient(azureDevopsOrgUri, credential);
        var workItemFactory = new WorkItemFactory(vssClient);
        return new WorkItemTrackingLogin(azureDevopsOrgUri, vssClient, workItemFactory);
    }
}
