namespace Develix.AzureDevOps.Connector.Model;

public record PackageVersion
{
    public string Version { get; init; } = "Unknown";
    public DateTime PublishDate { get; init; }
}
