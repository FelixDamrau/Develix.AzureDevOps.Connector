﻿using Develix.AzureDevOps.Connector.Service.Logic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal abstract class WorkItemTrackingRequestBase<T> : RequestBase<T, WorkItemTrackingLogin, WorkItemTrackingHttpClient>
{
    protected WorkItemTrackingRequestBase(WorkItemTrackingLogin? login)
        : base(login)
    {
    }
}
