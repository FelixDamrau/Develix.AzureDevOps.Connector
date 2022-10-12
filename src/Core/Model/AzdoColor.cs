namespace Develix.AzureDevOps.Connector.Model;

public class AzdoColor
{
    public AzdoColor(string colorString)
    {
        ColorString = colorString;
        Rgb = GetRgbString(colorString);
    }

    public string ColorString { get; }
    public string Rgb { get; }

    private static string GetRgbString(string colorString)
    {
        return colorString switch
        {
            { Length: 6 } => colorString,
            { Length: 8 } => colorString[0..6],
            _ => "FF0000",
        };
    }
}
