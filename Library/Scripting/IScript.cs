namespace BlocklyNet.Scripting;

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
}
