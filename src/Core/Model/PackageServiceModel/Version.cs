namespace Develix.AzureDevOps.Connector.Model.PackageServiceModel;

// This class was auto-generated from the GetAllPackages http response.
public class Version
{
    public Guid id { get; set; }
    public string normalizedVersion { get; set; } = null!;
    public string version { get; set; } = null!;
    public DateTime publishDate { get; set; }
}
