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
    /// Last exception observed - will not be reset.
    /// </summary>
    public Exception? LastException { get; set; }

    /// <summary>
    /// Initialize a new context.
    /// </summary>
    /// <param name="engine">Script engine to use.</param>
    /// <param name="variableTypes">Type of all variables.</param>
    public Context(IScriptSite engine, IDictionary<string, string>? variableTypes = null)
    {
        Engine = engine;
        VariableTypes = variableTypes ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Initialize a new context.
    /// </summary>
    /// <param name="parent">Parent context to use.</param>
    public Context(Context parent)
    {
        Engine = parent.Engine;
        Functions = parent.Functions;
        Parent = parent;
        VariableTypes = parent.VariableTypes;
    }

    /// <summary>
    /// The values of all variables.
    /// </summary>
    public IDictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();

    /// <summary>
    /// The types of all variables.
    /// </summary>
    public IDictionary<string, string> VariableTypes { get; private set; }

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

