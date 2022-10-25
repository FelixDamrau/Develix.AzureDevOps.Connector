namespace Develix.AzureDevOps.Connector.Model;

public class AreaPath
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string GetDisplayName()
    {
        var idx = Name.IndexOf(@"\Area");
        return (idx >= 0 && Name.FirstOrDefault() == '\\')
            ? Name[1..idx] + Name[(idx + 5)..] // remove first '\' and remove '\Area' classification node identifier
            : Name;
    }

    public static AreaPath Invalid { get; } = new() { Name = "Invalid" };
}
