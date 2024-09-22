using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Howto repeat an execution group from a former execution of a script.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GroupRepeatType
{
    /// <summary>
    /// May be nested execution information when parent is skipped.
    /// </summary>
    Unset = 0,

    /// <summary>
    /// Skip the execution and just use the result from the former run.
    /// </summary>
    Skip = 1,

    /// <summary>
    /// Forget the former result and run execute group again.
    /// </summary>
    Again = 2,
}
