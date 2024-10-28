using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Engine;
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
    public string JobId { get; } = Guid.NewGuid().ToString().ToUpper();

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
}

/// <summary>
/// Describes an active script.
/// </summary>
public abstract class Script<TOption>(TOption? options) : Script where TOption : StartScriptOptions
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
public abstract class Script<TRequest, TResult, TOption>(TRequest request, IScriptSite engine, TOption? options) : Script<TOption>(options) where TRequest : StartScript where TOption : StartScriptOptions
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
