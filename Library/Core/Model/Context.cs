using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Core.Model;

/// <summary>
/// Script execution context.
/// </summary>
public class Context
{
    /// <summary>
    /// Script engine running.
    /// </summary>
    public IScriptSite Engine { get; private set; }

    /// <summary>
    /// Provide access to the dependency injection.
    /// </summary>
    public IServiceProvider ServiceProvider => Engine.ServiceProvider;

    /// <summary>
    /// Control cancel of a script.
    /// </summary>
    public CancellationToken Cancellation => Engine.Cancellation;

    /// <summary>
    /// Initialize a new context.
    /// </summary>
    /// <param name="engine">Script engine to use.</param>
    public Context(IScriptSite engine)
    {
        Engine = engine;
    }

    /// <summary>
    /// Initialize a new context.
    /// </summary>
    /// <param name="parent">Parent context to use.</param>
    public Context(Context parent)
    {
        Parent = parent;
        Engine = parent.Engine;
        Functions = parent.Functions;
    }

    /// <summary>
    /// The values of all variables.
    /// </summary>
    public IDictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();

    /// <summary>
    /// All functions available in this context.
    /// </summary>
    public IDictionary<string, Statement> Functions { get; private set; } = new Dictionary<string, Statement>();

    /// <summary>
    /// How to leave loops.
    /// </summary>
    public EscapeMode EscapeMode { get; set; }

    /// <summary>
    /// Optional parent context - e.g. for the context of a currently exceuted function.
    /// </summary>
    public Context? Parent { get; private set; }

    /// <summary>
    /// In parallel mode the run script block will not evaluate but report itself.
    /// </summary>
    public int ParallelMode { get; set; }
}

