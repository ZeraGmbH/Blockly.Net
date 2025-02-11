using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;
using Microsoft.Extensions.DependencyInjection;

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

/// <summary>
/// Describes an active script.
/// </summary>
public abstract class Script<TOption, TLogType>(TOption? options) : Script
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult
{
    /// <inheritdoc/>
    public override TOption? Options => options;
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TOption"></typeparam>
/// <typeparam name="TLogType"></typeparam>
public abstract class Script<TRequest, TResult, TOption, TLogType>(TRequest request, IScriptSite engine, TOption? options) : Script<TOption, TLogType>(options)
    where TRequest : StartScript
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult
{
    /// <summary>
    /// 
    /// </summary>
    public TRequest Request { get; } = request;

    /// <inheritdoc/>
    public void SetResult(TResult result) => Result = result;

    /// <summary>
    /// 
    /// </summary>
    public override StartScript GetRequest() => Request;

    /// <summary>
    /// 
    /// </summary>
    public readonly IScriptSite Engine = engine;

    /// <summary>
    /// Test for cancel.
    /// </summary>
    protected void CheckCancel()
    {
        // Brute force cancel.
        Engine.Cancellation.ThrowIfCancellationRequested();

        // More soft pause.
        if (Engine.MustPause) throw new ScriptPausedException();
    }

    /// <summary>
    /// Access a runtime service.
    /// </summary>
    /// <typeparam name="T">Type of the service.</typeparam>
    /// <returns>Service as requested.</returns>
    protected T GetService<T>() where T : notnull => Engine.ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// Execute the script.
    /// </summary>
    protected abstract Task OnExecuteAsync();

    /// <inheritdoc/>
    public sealed override Task ExecuteAsync() => OnExecuteAsync();
}
