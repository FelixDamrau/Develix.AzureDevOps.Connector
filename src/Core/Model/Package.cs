namespace Develix.AzureDevOps.Connector.Model;

public record Package
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "Unknown";
    public IReadOnlyList<PackageVersion> Versions { get; init; } = new List<PackageVersion>();
}
