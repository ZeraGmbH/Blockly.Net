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
    public abstract Task Execute();

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
/// <param name="request"></param>
/// <param name="engine"></param>
public abstract class Script<TRequest, TResult, TOption>(TRequest request, IScriptSite engine, TOption? options) : Script<TOption>(options) where TRequest : StartScript where TOption : StartScriptOptions
{
    /// <summary>
    /// 
    /// </summary>
    public readonly TRequest Request = request;

    /// <inheritdoc/>
    protected void SetResult(TResult result) => Result = result;

    /// <summary>
    /// 
    /// </summary>
    public override StartScript GetRequest() => Request;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IScriptSite Engine = engine;

    /// <summary>
    /// Test for cancel.
    /// </summary>
    protected void CheckCancel() => Engine.Cancellation.ThrowIfCancellationRequested();

    /// <summary>
    /// Access a runtime service.
    /// </summary>
    /// <typeparam name="T">Type of the service.</typeparam>
    /// <returns>Service as requested.</returns>
    protected T GetService<T>() where T : notnull => Engine.ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// Execute the script.
    /// </summary>
    protected abstract Task OnExecute();

    /// <inheritdoc/>
    public sealed override Task Execute() => OnExecute();
}
