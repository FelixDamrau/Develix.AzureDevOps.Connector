namespace Develix.AzureDevOps.Connector.Model;

public class WorkItemType
{
    public WorkItemType(string name, string description, string color, string? iconUri)
    {
        Name = name;
        Description = description;
        Color = color;
        if (Uri.TryCreate(iconUri, UriKind.Absolute, out var uri))
            Icon = uri;
    }

    public string Name { get; }
    public string Description { get; }
    public string Color { get; }
    public Uri? Icon { get; }

    public static WorkItemType Invalid { get; } = new("Invalid", "An invalid work item type", "FF0000", null);
    public static WorkItemType Unknown { get; } = new("Unknown", "An unknown work item type", "808080", null);
}
