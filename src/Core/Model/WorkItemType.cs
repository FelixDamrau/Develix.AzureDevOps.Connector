using System.Drawing;

namespace Develix.AzureDevOps.Connector.Model;

public enum WorkItemType
{
    [Display("Invalid", "???", KnownColor.Magenta)]
    Invalid = 0,

    [Display("Bug", "Bug", KnownColor.Red)]
    Bug,

    [Display("Epic", "Epi", KnownColor.Orange)]
    Epic,

    [Display("Feature", "Fea", KnownColor.Lavender)]
    Feature,

    [Display("Impediment", "Imp", KnownColor.DarkMagenta)]
    Impediment,

    [Display("Prodcut Backlog Item", "PBI", KnownColor.Cyan)]
    ProductBacklogItem,

    [Display("Task", "Tas", KnownColor.Yellow)]
    Task,

    [Display("Unknown", "???", KnownColor.Gray)]
    Unknown,
}
