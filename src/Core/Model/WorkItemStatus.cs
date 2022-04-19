using System.Drawing;

namespace Develix.AzureDevOps.Connector.Model;

public enum WorkItemStatus
{
    [Display("Invalid", "???", KnownColor.Magenta)]
    Invalid = 0,

    [Display("New", "New", KnownColor.Red)]
    New,

    [Display("Approved", "App", KnownColor.Yellow)]
    Approved,

    [Display("Committed", "Com", KnownColor.Yellow)]
    Committed,

    [Display("Done", "Don", KnownColor.Green)]
    Done,

    [Display("Removed", "Del", KnownColor.Green)]
    Removed,

    [Display("In Progress", "Pro", KnownColor.Yellow)]
    InProgress,

    [Display("Open", "Ope", KnownColor.Yellow)]
    Open,

    [Display("Closed", "Clo", KnownColor.Green)]
    Closed,

    [Display("To do", "Tdo", KnownColor.Yellow)]
    ToDo
}
