using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Logging;

/// <summary>
/// Possible outcomes of a script.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScriptExecutionResultTypes
{
    /// <summary>
    /// Measurement was successfull.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Measurement failed.
    /// </summary>
    Failure = 1,

    /// <summary>
    /// Measurement was aborted before it could finish.
    /// </summary>
    Aborted = 2,

    /// <summary>
    /// Measurement is currently active.
    /// </summary>
    Active = 3,

    /// <summary>
    /// Measurement terminated due to some error condition.
    /// </summary>
    Error = 4,

    /// <summary>
    /// Measurement has been paused and may be restarted.
    /// </summary>
    [Obsolete("no longer used - only Aborted is supported", false)]
    Paused = 5,
}