using System.Diagnostics.CodeAnalysis;
using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Develix.AzureDevOps.Connector.Service;

public abstract class VssService<TVssClient, TLogin> : IAzureDevOpsService, IDisposable
    where TVssClient : class, IDisposable
    where TLogin : AzureDevopsLogin<TVssClient>
{
    protected TLogin? azureDevopsLogin;

    public virtual async Task<Result> Initialize(Uri azureDevopsOrgUri, string azureDevopsWorkItemReadToken)
    {
        azureDevopsLogin = null;
        var result = await Wrap(async () => await CreateLogin(azureDevopsOrgUri, azureDevopsWorkItemReadToken).ConfigureAwait(false)).ConfigureAwait(false);
        if (result.Valid)
        {
            azureDevopsLogin = result.Value;
            return Result.Ok();
        }
        return Result.Fail(result.Message);
    }

    [MemberNotNullWhen(true, nameof(azureDevopsLogin))]
    public bool IsInitialized() => azureDevopsLogin is not null;

    [MemberNotNull(nameof(azureDevopsLogin))]
    protected void IsInitializedGuard()
    {
        if (!IsInitialized())
            throw new InvalidOperationException($"The services are not initialzed!");
    }

    protected abstract Task<TLogin> CreateLogin(Uri baseUri, string azureDevopsWorkItemReadToken);

    protected async Task<Result<T>> Wrap<T>(Func<Task<T>> function)
    {
        try
        {
            var response = await function.Invoke().ConfigureAwait(false);
            return Result.Ok(response);
        }
        catch (VssUnauthorizedException e)
        {
            return Result.Fail<T>("Authorization failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (VssServiceResponseException e)
        {
            return Result.Fail<T>("Connection failed!" + Environment.NewLine + "Error message: " + e.Message);
        }
        catch (Exception e)
        {
            return Result.Fail<T>("Unknown error!" + Environment.NewLine + "Error message: " + e.Message);
        }
    }

    #region IDisposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                azureDevopsLogin?.VssClient?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
