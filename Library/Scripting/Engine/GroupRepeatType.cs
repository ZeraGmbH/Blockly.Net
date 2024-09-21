using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Howto repeat an execution group from a former execution of a script.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GroupRepeatType
{
    /// <summary>
    /// Skip the execution and just use the result from the former run.
    /// </summary>
    Skip = 0,

    /// <summary>
    /// Forget the former result and run execute group again.
    /// </summary>
    Again = 1,
}
