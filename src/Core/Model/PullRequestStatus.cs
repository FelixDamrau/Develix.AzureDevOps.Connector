using System.Drawing;

namespace Develix.AzureDevOps.Connector.Model;

public enum PullRequestStatus
{
    [Display("Invalid", "???", KnownColor.Magenta)]
    Invalid = 0,

    [Display("Active", "Act", KnownColor.Red)]
    Active,

    [Display("Abandoned", "Abd", KnownColor.Green)]
    Abandoned,

    [Display("Completed", "Don", KnownColor.Green)]
    Completed
}
