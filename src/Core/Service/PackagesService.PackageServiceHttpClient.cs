using System.Text.Json;
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

        public async Task<ResponseObject> GetAllPackages(string project, string feed)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = GetRequestUri(project, feed),
                Method = HttpMethod.Get,
            };

            var response = await httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResponseObject>(result) ?? new();

            Uri GetRequestUri(string project, string feed)
                => new($"{baseUri}{project}/_apis/packaging/Feeds/{feed}/packages?includeAllVersions=true&api-version=6.0-preview.1");
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
