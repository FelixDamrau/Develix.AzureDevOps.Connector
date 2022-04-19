using System.Drawing;

namespace Develix.AzureDevOps.Connector.Model;

public enum TrackingBranchStatus
{
    [Display("Invalid", "???", KnownColor.Magenta)]
    Invalid = 0,

    [Display("None", "Non", KnownColor.Gray)]
    None,

    [Display("Active", "Act", KnownColor.Green)]
    Active,

    [Display("Deleted", "Del", KnownColor.Red)]
    Deleted
}
