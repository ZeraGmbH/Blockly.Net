namespace BlocklyNet.Scripting.Logging;

/// <summary>
/// 
/// </summary>
public interface IScriptLogModifier
{
    /// <summary>
    /// Map library blind view of script implemenation type
    /// to simplify coding log modifiers.
    /// </summary>
    /// <param name="script">Script to which this modifier belongs.</param>
    /// <param name="initial">Unset if this is a replay of the modification.</param>
    Task ApplyAsync(Script script, bool initial);
}
