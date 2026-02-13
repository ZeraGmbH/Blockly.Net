using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// Describes an active script.
/// </summary>
/// <typeparam name="TOption">Option class to use.</typeparam>
/// <typeparam name="TLogType">Type of each log entry.</typeparam>
/// <typeparam name="TModifierType">Implementation class of log modifiers.</typeparam>
public abstract class Script<TOption, TLogType, TModifierType> : Script, IScriptInstance<TLogType>, IScript<TLogType>
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult, new()
    where TModifierType : ScriptLogModifier
{
    private readonly TOption? _options;

    /// <summary>
    /// Must only be created inside this assembly.
    /// </summary>
    /// <param name="options">Options to use.</param>
    public Script(TOption? options)
    {
        _options = options;

        ResultForLogging = CreateResultForLogging();
    }

    /// <inheritdoc/>
    public override TOption? Options => _options;

    /// <inheritdoc/>
    public TLogType ResultForLogging { get; set; }

    /// <summary>
    /// Create a brand new empty log entry.
    /// </summary>
    /// <returns>Empty log entry.</returns>
    protected virtual TLogType CreateResultForLogging() => new();

    /// <summary>
    /// Identifier of the last log created in no-log
    /// operation mode - pure imaginary and more or
    /// less useless.
    /// </summary>
    private string? _lastLogId;

    /// <summary>
    /// Internal reset of state.
    /// </summary>
    protected virtual Task ReinitializeAsync()
    {
        ResultForLogging = CreateResultForLogging();

        _ActiveGroups.Clear();
        _Modifiers.Clear();

        _lastLogId = null;

        return Task.CompletedTask;
    }

    /// <summary>
    /// All side-effects applied to the log entry during
    /// script execution - not used in no-log base 
    /// operation mode.
    /// </summary>
    private readonly List<TModifierType> _Modifiers = [];

    /// <summary>
    /// Register a side-effect.
    /// </summary>
    /// <param name="modifier">Side effect to apply to the
    /// log entry and remember for replay.</param>
    protected Task RegisterModifierAsync(TModifierType modifier)
    {
        // Remember for later replay and execute now.
        _Modifiers.Add(modifier);

        return modifier.ApplyAsync(this, true);
    }

    /// <summary>
    /// Wrapper class for replay information on side-effects
    /// on the log entry.
    /// </summary>
    private class CustomGroupInformation
    {
        /// <summary>
        /// Number of modfiers prior to these ones.
        /// </summary>
        public int ModifierIndex { get; set; }

        /// <summary>
        /// List of modifiers for a single execution group.
        /// </summary>
        public List<TModifierType> Modifiers { get; set; } = [];
    }

    /// <summary>
    /// Replay information for each execution group.
    /// </summary>
    private readonly Stack<CustomGroupInformation> _ActiveGroups = [];

    /// <summary>
    /// Decode list of modifiers on this script.
    /// </summary>
    /// <param name="status">Some status information.</param>
    /// <returns>List of modifiers.</returns>
    protected IEnumerable<TModifierType> GetModifiers(GroupStatus status)
        => BlockyUtils.Decompress<CustomGroupInformation>(status.CustomizerBlob)?.Modifiers ?? Enumerable.Empty<TModifierType>();

    /// <inheritdoc/>
    public override async Task BeginGroupExecutionAsync(GroupStatus status, bool repeat)
    {
        if (repeat)
            foreach (var modifier in GetModifiers(status))
                await modifier.ApplyAsync(this, false);
        else
            _ActiveGroups.Push(new() { ModifierIndex = _Modifiers.Count });
    }

    /// <inheritdoc/>
    public override Task EndGroupExecutionAsync(GroupStatus status)
    {
        /* Get the side effects create by this group. */
        var info = _ActiveGroups.Pop();

        info.Modifiers.AddRange(_Modifiers.Skip(info.ModifierIndex));

        /* Serialize in group. */
        status.CustomizerBlob = BlockyUtils.Compress(info);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override void SetGroups(ScriptGroupStatus? status)
        => ResultForLogging.GroupsFinished = status;

    /// <inheritdoc/>
    public virtual Task SetResultAsync(ScriptExecutionResultTypes result)
    {
        /* In no-log mode just update the entry. */
        ResultForLogging.Result = result;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task<string> WriteToLogAsync()
    {
        /* In no-log mode just generate and keep a unique identifier for the log entry. */
        if (string.IsNullOrEmpty(_lastLogId)) _lastLogId = Guid.NewGuid().ToString();

        return Task.FromResult(_lastLogId);
    }

    /// <inheritdoc/>
    public virtual Task RegisterChildAsync(string id)
    {
        /* In no-log mode just remember the log entry in the hierarchy of nested scripts. */
        ResultForLogging.Children.Add(id);

        return Task.CompletedTask;
    }
}
