namespace Develix.AzureDevOps.Connector.Model;

public class WorkItemStatus
{
    public WorkItemStatus(string name, string color, string category)
    {
        Name = name;
        Color = new(color);
        Category = category;
    }

    public string Name { get; }
    public AzdoColor Color { get; set; }
    public string Category { get; }

    public static WorkItemStatus Unknown { get; } = new("Unknown", "FF0000", "Unknown");
    public static WorkItemStatus Invalid { get; } = new("Invalid", "FF0000", "Invalid");
}
