using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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

    public static async Task<WorkItemTrackingLogin> Create(Uri azureDevopsOrgUri, string token)
    {
        var credential = new VssBasicCredential(string.Empty, token);
        var vssClient = new WorkItemTrackingHttpClient(azureDevopsOrgUri, credential);
        await ConnectionCheck(vssClient);
        var workItemFactory = new WorkItemFactory(vssClient);
        return new WorkItemTrackingLogin(azureDevopsOrgUri, vssClient, workItemFactory);
    }

    private static async Task ConnectionCheck(WorkItemTrackingHttpClient client)
    {
        var wiql = new Wiql() { Query = "Select [Id] From WorkItems Where [Id] = 367" };
        await client.QueryByWiqlAsync(wiql);
    }
}
