using System.Drawing;

namespace Develix.AzureDevOps.Connector.Model;

[AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = false)]
public class DisplayAttribute : Attribute
{
    public DisplayAttribute(string name, string shortName, KnownColor color)
    {
        Name = name;
        ShortName = shortName;
        Color = Color.FromKnownColor(color);
    }

    public string Name { get; }
    public string ShortName { get; }
    public Color Color { get; }
}
