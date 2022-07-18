﻿using Develix.AzureDevOps.Connector.Service;
using Fluxor;

namespace Develix.AzureDevOps.Connector.App.Store.AzureDevOpsServicesUseCase;

public class Effects
{
    public Effects(IReposService reposService, IWorkItemService workItemService)
    {
        ReposService = reposService ?? throw new ArgumentNullException(nameof(reposService));
        WorkItemService = workItemService ?? throw new ArgumentNullException(nameof(workItemService));
    }

    public IReposService ReposService { get; set; }
    public IWorkItemService WorkItemService { get; set; }

    [EffectMethod]
    public async Task HandleLoginRepoServiceAction(LoginRepoServiceAction action, IDispatcher dispatcher)
    {
        var uri = GetUriWithTrailingSlash(action.AzureDevopsOrgUri);
        var login = await ReposService.Initialize(uri, action.Token);
        var resultAction = new LoginRepoServiceResultAction(login.Valid ? Model.ConnectionStatus.Connected : Model.ConnectionStatus.NotConnected);
        dispatcher.Dispatch(resultAction);
    }

    [EffectMethod]
    public async Task HandleLoginWorkItemServiceAction(LoginWorkItemServiceAction action, IDispatcher dispatcher)
    {
        var uri = GetUriWithTrailingSlash(action.AzureDevopsOrgUri);
        var login = await WorkItemService.Initialize(uri, action.Token);
        var resultAction = new LoginWorkItemServiceResultAction(login.Valid ? Model.ConnectionStatus.Connected : Model.ConnectionStatus.NotConnected);
        dispatcher.Dispatch(resultAction);
    }

    /// <summary>
    /// Some features expect the absolute uri to end with an slash. So we normalize all azure devops organization uris to include the trailing slash.
    /// </summary>
    private static Uri GetUriWithTrailingSlash(Uri uri)
    {
        if (uri.AbsoluteUri[^1] == '/')
            return uri;
        return new Uri(uri.AbsoluteUri + '/');
    }
}
