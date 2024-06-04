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
    public HashSet<int> Values { get; set; } = [];

    /// <summary>
    /// Assign any action to this <see cref="EventCallback"/>, this will cause the parent control to fire <see cref="ComponentBase.StateHasChanged"/> when the action is triggered.
    /// </summary>
    [Parameter]
    public EventCallback OnValuesChange { get; set; }

    [MemberNotNullWhen(false, nameof(intValue))]
    private bool Disabled() => intValue is null || Values.Contains(intValue.Value);

    private async Task AddAsync()
    {
        if (Disabled())
            return;
        Values.Add(intValue.Value);
        await OnValuesChange.InvokeAsync().ConfigureAwait(true);
        if(numericField is not null)
            await numericField.ResetAsync().ConfigureAwait(true);
    }

    private async Task CloseAsync(MudChip chip)
    {
        Values.Remove((int)chip.Value);
        await OnValuesChange.InvokeAsync().ConfigureAwait(true);
    }
}
