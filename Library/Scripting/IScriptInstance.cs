using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// Represents a running script.
/// </summary>
public interface IScriptInstance : IScript
{
    /// <summary>
    /// Untyped result of the script.
    /// </summary>
    object? Result { get; }

    /// <summary>
    /// Start the script execution - once.
    /// </summary>
    Task ExecuteAsync();

    /// <summary>
    /// Report the configuration used to start the script.
    /// </summary>
    StartScript GetRequest();
}

/// <summary>
/// Logging ready description of a running script.
/// </summary>
/// <typeparam name="TLogType">Type of a log entry.</typeparam>
public interface IScriptInstance<TLogType> : IScript<TLogType>, IScriptInstance where TLogType : ScriptLoggingResult, new()
{
}
