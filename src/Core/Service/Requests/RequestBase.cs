using Develix.AzureDevOps.Connector.Service.Logic;
using Develix.Essentials.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Develix.AzureDevOps.Connector.Service.Requests;

internal abstract class RequestBase<T, TLogin, TVssClient>
    where TVssClient : class, IDisposable
    where TLogin : AzureDevopsLogin<TVssClient>
{
    private readonly TLogin? login;

    public RequestBase(TLogin? login)
    {
        this.login = login;
    }

    public async Task<Result<T>> Execute(CancellationToken cancellationToken = default)
    {
        if (login is null)
            return Result.Fail<T>("Service is not initialized!");
        try
        {
            var response = await Execute(login, cancellationToken).ConfigureAwait(false);
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

    protected abstract Task<T> Execute(TLogin login, CancellationToken cancellationToken = default);
}
