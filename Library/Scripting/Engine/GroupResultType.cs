using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// States of a execution group.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GroupResultType
{
    /// <summary>
    /// Group started execution but did not yet finish.
    /// </summary>
    Running = 0,

    /// <summary>
    /// Group execution finished successfull.
    /// </summary>

    Success = 1,

    /// <summary>
    /// Group execution failed with some error.
    /// </summary>
    Failed = 2
}
