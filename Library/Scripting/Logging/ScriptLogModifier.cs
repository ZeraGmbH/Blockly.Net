namespace BlocklyNet.Scripting.Logging;

/// <summary>
/// Base class to implement log entry modifiers.
/// </summary>
public abstract class ScriptLogModifier
{
    /// <summary>
    /// Map library blind view of script implemenation type
    /// to simplify coding log modifiers.
    /// </summary>
    /// <param name="script">Script to which this modifier belongs.</param>
    /// <param name="initial">Unset if this is a replay of the modification.</param>    
    public abstract Task ApplyAsync(IScript script, bool initial);
}

/// <summary>
/// Base class to implement log entry modifiers.
/// </summary>
/// <typeparam name="TScript">Concrete interface to use.</typeparam>
public abstract class ScriptLogModifier<TScript> : ScriptLogModifier where TScript : IScript
{
    /// <inheritdoc/>
    public override sealed Task ApplyAsync(IScript script, bool initial) => OnApplyAsync((TScript)script, initial);

    /// <summary>
    /// Apply this modification to a script.
    /// </summary>
    /// <param name="script">The correspondig script.</param>
    /// <param name="initial">Set if the log modifier is just created during script execution.</param>
    protected abstract Task OnApplyAsync(TScript script, bool initial);
}
