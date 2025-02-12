using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Scripting;

/// <summary>
/// Describes an active script.
/// </summary>
public abstract class Script : IScriptInstance, IScript
{
    /// <summary>
    /// The unique identifier of the active script.
    /// </summary>
    public string JobId { get; private set; } = Guid.NewGuid().ToString().ToUpper();

    /// <inheritdoc/>
    public object? Result { get; protected set; }

    /// <inheritdoc/>
    public abstract Task ExecuteAsync();

    /// <inheritdoc/>
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

        // Derived class.
        await OnResetAsync();
    }

    /// <summary>
    /// Call to do some internal reset of the script.
    /// </summary>
    protected virtual Task OnResetAsync() => throw new NotSupportedException($"can not restart {GetType().FullName}");

    /// <summary>
    /// Report the start of a group execution.
    /// </summary>
    /// <param name="status">Status pf the group.</param>
    /// <param name="repeat">Set if this is a repeat operation and results should be recovered.</param>
    public abstract Task BeginGroupExecutionAsync(GroupStatus status, bool repeat);

    /// <summary>
    /// A group has finished execution - not called for repeat.
    /// </summary>
    /// <param name="status">Status of the completed group.</param>
    public abstract Task EndGroupExecutionAsync(GroupStatus status);

    /// <inheritdoc/>
    public abstract void SetGroups(ScriptGroupStatus? status);

    /// <inheritdoc/>
    StartScript IScript.Request => GetRequest();
}
