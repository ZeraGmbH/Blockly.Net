using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Scripting;

/// <summary>
/// 
/// </summary>
public interface IScript
{
    /// <summary>
    /// The unique identifier of the active script.
    /// </summary>
    string JobId { get; }

    /// <summary>
    /// Can be used to check for early termination.
    /// </summary>
    StartScriptOptions? Options { get; }

    /// <summary>
    /// Set group information of the script.
    /// </summary>
    /// <param name="status">Group information collected.</param>
    void SetGroups(ScriptGroupStatus? status);

    /// <summary>
    /// Report the related request.
    /// </summary>
    StartScript Request { get; }

}
