using System.Text.Json;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// Describes an active script.
/// </summary>
public abstract class Script : IScriptInstance
{
    /// <summary>
    /// The unique identifier of the active script.
    /// </summary>
    public string JobId { get; private set; } = Guid.NewGuid().ToString().ToUpper();

    /// <summary>
    /// Untyped result of the script.
    /// </summary>
    public object? Result { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract Task ExecuteAsync();

    /// <summary>
    /// 
    /// </summary>
    public abstract StartScript GetRequest();

    /// <summary>
    /// Can be used to check for early termination.
    /// </summary>
    public abstract StartScriptOptions? Options { get; }

    /// <summary>
    /// Call to do some internal reset of the script.
    /// </summary>
    public async Task ResetAsync()
    {
        // Self.
        JobId = Guid.NewGuid().ToString().ToUpper();
        Result = null;

        // Derived clas.
        await OnResetAsync();
    }

    /// <summary>
    /// Internal reset of state.
    /// </summary>
    protected virtual Task ReinitializeAsync()
    {
        _ActiveGroups.Clear();
        _Modifiers.Clear();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Call to do some internal reset of the script.
    /// </summary>
    protected virtual Task OnResetAsync() => throw new NotSupportedException($"can now restart {GetType().FullName}");

    private readonly List<ScriptLogModifier> _Modifiers = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modifier"></param>
    protected Task RegisterModifierAsync(ScriptLogModifier modifier)
    {
        // Remember for later replay and execute now.
        _Modifiers.Add(modifier);

        return modifier.ApplyAsync(this, true);
    }

    private class CustomGroupInformation(int modifierIndex)
    {
        public int ModifierIndex { get; set; } = modifierIndex;

        public List<ScriptLogModifier> Modifiers { get; set; } = [];
    }

    private readonly Stack<CustomGroupInformation> _ActiveGroups = [];

    /// <summary>
    /// Report the start of a group execution.
    /// </summary>
    /// <param name="status">Status pf the group.</param>
    /// <param name="repeat">Set if this is a repeat operation and results should be recovered.</param>
    public async Task BeginGroupExecutionAsync(GroupStatus status, bool repeat)
    {
        if (repeat)
        {
            /* Reconstruct extra information. */
            var info = JsonSerializer.Deserialize<CustomGroupInformation>(status.CustomizerBlob!, JsonUtils.JsonSettings)!;

            /* Re-apply all side effects on the logging. */
            foreach (var modifier in info.Modifiers)
                await modifier.ApplyAsync(this, false);
        }
        else
            _ActiveGroups.Push(new(_Modifiers.Count));
    }

    /// <summary>
    /// A group has finished execution - not called for repeat.
    /// </summary>
    /// <param name="status">Status of the completed group.</param>
    public Task EndGroupExecutionAsync(GroupStatus status)
    {
        /* Get the side effects create by this group. */
        var info = _ActiveGroups.Pop();

        info.Modifiers.AddRange(_Modifiers.Skip(info.ModifierIndex));

        /* Serialize in group. */
        status.CustomizerBlob = JsonSerializer.Serialize(info, JsonUtils.JsonSettings);

        return Task.CompletedTask;
    }
}
