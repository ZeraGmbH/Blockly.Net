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
    /// Call to do some internal reset of the script.
    /// </summary>
    protected virtual Task OnResetAsync() => throw new NotSupportedException($"can now restart {GetType().FullName}");

}
