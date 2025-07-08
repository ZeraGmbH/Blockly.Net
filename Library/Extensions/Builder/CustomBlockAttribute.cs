namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Used to mark a self-describing script block with meta-data.
/// </summary>
/// <param name="key">Unique block key.</param>
/// <param name="category">Toolbox category - leave empty to place on top level.</param>
/// <param name="definition">Block definition.</param>
/// <param name="toolbox">Optional toolbox defaults for the block.</param>
[AttributeUsage(AttributeTargets.Class)]
public class CustomBlockAttribute(string key, string category, string definition, string toolbox) : Attribute
{
    /// <summary>
    /// Unique block key.
    /// </summary>
    public readonly string Key = key;

    /// <summary>
    /// Toolbox category - leave empty to place on top level.
    /// </summary>
    public readonly string Category = category;

    /// <summary>
    /// Block definition.
    /// </summary>
    public readonly string Definition = definition;

    /// <summary>
    /// Optional toolbox defaults for the block.
    /// </summary>
    public readonly string Toolbox = toolbox;

    /// <summary>
    /// Block is hidden from customized area - must be
    /// provided in some other way or just be internal.
    /// </summary>
    public bool Hidden = false;
}
