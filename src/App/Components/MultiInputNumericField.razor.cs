using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Develix.AzureDevOps.Connector.App.Components;

public partial class MultiInputNumericField
{
    private int? intValue;
    private MudNumericField<int?>? numericField;

    [Parameter]
    public string ChipPrefix { get; set; } = "#";

    [Parameter]
    public string Label { get; set; } = "Number";

    [Parameter]
    public HashSet<int> Values { get; set; } = new();

    /// <summary>
    /// Assign any action to this <see cref="EventCallback"/>, this will cause the parent control to fire <see cref="ComponentBase.StateHasChanged"/> when the action is triggered.
    /// </summary>
    [Parameter]
    public EventCallback OnValuesChange { get; set; }

    [MemberNotNullWhen(false, nameof(intValue))]
    private bool Disabled() => intValue is null || Values.Contains(intValue.Value);

    private void Add()
    {
        if (Disabled())
            return;
        Add(intValue.Value);
        numericField?.Reset();
    }

    private void Add(int value)
    {
        Values.Add(value);
        OnValuesChange.InvokeAsync();
    }

    private void Closed(MudChip chip)
    {
        Values.Remove((int)chip.Value);
        OnValuesChange.InvokeAsync();
    }
}
