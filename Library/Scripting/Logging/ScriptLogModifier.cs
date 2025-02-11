namespace BlocklyNet.Scripting.Logging;

/// <summary>
/// 
/// </summary>
public abstract class ScriptLogModifier
{
    /// <summary>
    /// Map library blind view of script implemenation type
    /// to simplify coding log modifiers.
    /// </summary>
    /// <param name="script">Script to which this modifier belongs.</param>
    /// <param name="initial">Unset if this is a replay of the modification.</param>
    public abstract Task ApplyAsync(Script script, bool initial);
}
