using System.Text.Json;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// Describes an active script.
/// </summary>
public abstract class Script<TOption, TLogType, TModifierType> : Script
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult, new()
    where TModifierType : ScriptLogModifier, new()
{
    private readonly TOption? _options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public Script(TOption? options)
    {
        _options = options;

        ResultForLogging = CreateResultForLogging();
    }

    /// <inheritdoc/>
    public override TOption? Options => _options;

    /// <summary>
    /// 
    /// </summary>
    public TLogType ResultForLogging { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual TLogType CreateResultForLogging() => new();

    /// <summary>
    /// Internal reset of state.
    /// </summary>
    protected virtual Task ReinitializeAsync()
    {
        ResultForLogging = CreateResultForLogging();

        _ActiveGroups.Clear();
        _Modifiers.Clear();

        return Task.CompletedTask;
    }

    private readonly List<TModifierType> _Modifiers = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modifier"></param>
    protected Task RegisterModifierAsync(TModifierType modifier)
    {
        // Remember for later replay and execute now.
        _Modifiers.Add(modifier);

        return modifier.ApplyAsync(this, true);
    }

    /// <summary>
    /// 
    /// </summary>
    private class CustomGroupInformation
    {
        public int ModifierIndex { get; set; }

        public List<TModifierType> Modifiers { get; set; } = [];
    }

    private readonly Stack<CustomGroupInformation> _ActiveGroups = [];

    /// <summary>
    /// Report the start of a group execution.
    /// </summary>
    /// <param name="status">Status pf the group.</param>
    /// <param name="repeat">Set if this is a repeat operation and results should be recovered.</param>
    public async Task BeginGroupExecutionAsync(GroupStatus status, bool repeat)
    {
        if (repeat)
        {
            /* Reconstruct extra information. */
            var info = JsonSerializer.Deserialize<CustomGroupInformation>(status.CustomizerBlob!, JsonUtils.JsonSettings)!;

            /* Re-apply all side effects on the logging. */
            foreach (var modifier in info.Modifiers)
                await modifier.ApplyAsync(this, false);
        }
        else
            _ActiveGroups.Push(new() { ModifierIndex = _Modifiers.Count });
    }

    /// <summary>
    /// A group has finished execution - not called for repeat.
    /// </summary>
    /// <param name="status">Status of the completed group.</param>
    public Task EndGroupExecutionAsync(GroupStatus status)
    {
        /* Get the side effects create by this group. */
        var info = _ActiveGroups.Pop();

        info.Modifiers.AddRange(_Modifiers.Skip(info.ModifierIndex));

        /* Serialize in group. */
        status.CustomizerBlob = JsonSerializer.Serialize(info, JsonUtils.JsonSettings);

        return Task.CompletedTask;
    }
}
