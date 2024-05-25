namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Describe a model block.
/// </summary>
public interface IScriptModelInfo
{
    /// <summary>
    /// Display name of the model.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// .NET type of the model.
    /// </summary>
    Type Type { get; }
}
