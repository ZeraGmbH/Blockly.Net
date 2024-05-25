using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting;

/// <summary>
/// Base class for the parameters needed to start a new script.
/// </summary>
public abstract class StartScript
{
    /// <summary>
    /// Name of the script - the type of the script is defined by
    /// the type of derived classes.
    /// </summary>
    [Required, NotNull]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Report the type of the script.
    /// </summary>
    /// <returns>The type of the script.</returns>
    public abstract Type GetScriptType();

    /// <summary>
    /// Report the model used for this script.
    /// </summary>
    /// <returns>Unique name of the model.</returns>
    public abstract string ModelType { get; }
}

/// <summary>
/// Describes a specific type of script.
/// </summary>
public abstract class StartScript<TScript, TResult> : StartScript where TScript : Script
{
    /// <inheritdoc/>
    public override Type GetScriptType() => typeof(TScript);
}
