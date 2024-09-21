namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Former result of a group execution and an option for retry.
/// </summary>
public class GroupRepeat : GroupStatus<GroupRepeat>
{
    /// <summary>
    /// How to execute the group.
    /// </summary>
    public GroupRepeatType Repeat { get; set; }
}