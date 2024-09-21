using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// States of a execution group.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GroupResultType
{
    /// <summary>
    /// Some enumeration default - should never ever been seen but better
    /// be safe than sorry.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Group execution finished successfull.
    /// </summary>

    Succeeded = 1,

    /// <summary>
    /// Group execution failed with some error.
    /// </summary>
    Failed = 2
}
