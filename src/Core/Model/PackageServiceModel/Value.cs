namespace Develix.AzureDevOps.Connector.Model.PackageServiceModel;

// This class was auto-generated from the GetAllPackages http response.
public class Value
{
    public Guid id { get; set; }
    public string normalizedName { get; set; } = null!;
    public string name { get; set; } = null!;
    public Version[] versions { get; set; } = null!;
}
