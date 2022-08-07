using System.Text.Json;
using Develix.AzureDevOps.Connector.Model;
using Develix.Essentials.Core;

namespace Develix.AzureDevOps.Connector.Service;

public partial class PackagesService
{
    private class PackageServiceHttpClient
    {
        private readonly string baseUri;
        private readonly HttpClient httpClient;

        public PackageServiceHttpClient(string baseUri, string token)
        {
            this.baseUri = baseUri;
            httpClient = new HttpClient();
            var base64EncodedToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"token:{token}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64EncodedToken}");
        }

        public async Task<Result<ResponseObject>> GetAllPackages(string project, string feed)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = GetRequestUri(project, feed),
                Method = HttpMethod.Get,
            };

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<ResponseObject>(result) ?? new();
                return Result.Ok(responseObject);
            }
            return Result.Fail<ResponseObject>((response.ReasonPhrase ?? "No Reason given") + $" ({(int)response.StatusCode})");

            Uri GetRequestUri(string project, string feed)
                => new($"{baseUri}{project}/_apis/packaging/Feeds/{feed}/packages?includeAllVersions=true&api-version=6.0-preview.1");
        }

        public async Task<Result<Value>> GetPackage(string project, string feed, string packageName)
        {
            var packageIdResult = await GetPackageId(project, feed, packageName);
            if (!packageIdResult.Valid)
                return Result.Fail<Value>(packageIdResult.Message);

            var request = new HttpRequestMessage
            {
                RequestUri = GetRequestUri(project, feed, packageIdResult.Value),
                Method = HttpMethod.Get,
            };

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Value>(result) ?? new();
                return Result.Ok(responseObject);
            }
            return Result.Fail<Value>((response.ReasonPhrase ?? "No Reason given") + $" ({(int)response.StatusCode})");

            Uri GetRequestUri(string project, string feed, Guid packageId)
                => new($"{baseUri}{project}/_apis/packaging/Feeds/{feed}/packages/{packageId}/?includeAllVersions=true&api-version=6.0-preview.1");
        }

        private async Task<Result<Guid>> GetPackageId(string project, string feed, string packageName)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = GetRequestUri(project, feed, packageName),
                Method = HttpMethod.Get,
            };

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<ResponseObject>(result);

                if(responseObject is null)
                    return Result.Fail<Guid>($"Could not deserialize the httpResponse: [{result}]");

                return responseObject.count switch
                {
                    < 1 => Result.Fail<Guid>($"No packages with name '{packageName}' were found on feed '{feed}' of project '{project}'."),
                    1 => Result.Ok(responseObject.value.Single().id),
                    > 1 => Result.Fail<Guid>($"Multiple ({string.Join(",", responseObject.value.Select(p => p.normalizedName))}) packages with name '{packageName}' were found on feed '{feed}' of project '{project}'."),
                };
            }
            return Result.Fail<Guid>((response.ReasonPhrase ?? "No Reason given") + $" ({(int)response.StatusCode})");

            Uri GetRequestUri(string project, string feed, string packageName)
                => new($"{baseUri}{project}/_apis/packaging/Feeds/{feed}/packages/?packageNameQuery={packageName}&api-version=6.0-preview.1");
        }

        /// <summary>
        /// This sends a small request to test if the connection is successful.
        /// </summary>
        public async Task<Result> IsInitialized()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new($"{baseUri}_apis/packaging/feeds?api-version=6.0-preview.1"),
                Method = HttpMethod.Get,
            };
            var result = await httpClient.SendAsync(request);
            return result.IsSuccessStatusCode ? Result.Ok() : Result.Fail(result.ReasonPhrase ?? "Error unknown");
        }
    }

    // These classes were auto-generated from the GetAllPackages http response.
    private class ResponseObject
    {
        public int count { get; set; }
        public Value[] value { get; set; } = null!;
    }

    private class Value
    {
        public Guid id { get; set; }
        public string normalizedName { get; set; } = null!;
        public string name { get; set; } = null!;
        public Version[] versions { get; set; } = null!;
    }

    private class Version
    {
        public Guid id { get; set; }
        public string normalizedVersion { get; set; } = null!;
        public string version { get; set; } = null!;
        public DateTime publishDate { get; set; }
    }
}
