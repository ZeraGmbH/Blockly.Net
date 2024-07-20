namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Helper class to manage the block building process
/// /// </summary>
public class ModelCache
{
    /// <summary>
    /// All defined blocks.
    /// </summary>
    private readonly Dictionary<Type, string> _mapping = [];

    /// <summary>
    /// Check if a specific model is already known.
    /// </summary>
    /// <param name="type">Some model type.</param>
    /// <returns>Set if the model type is already known.</returns>
    public bool Contains(Type type) => _mapping.ContainsKey(type);

    /// <summary>
    /// Read a single mapping.
    /// </summary>
    /// <param name="type">Some model type.</param>
    public string this[Type type] => _mapping[type];

    /// <summary>
    /// Register an additional block type.
    /// </summary>
    /// <param name="name">Blockly type name for the model.</param>
    /// <typeparam name="T">Model type.</typeparam>
    public void Add<T>(string name) => Add(typeof(T), name);

    /// <summary>
    /// Register an additional block type.
    /// </summary>
    /// <param name="type">Model type.</param>
    /// <param name="name">Blockly type name for the model.</param>
    public void Add(Type type, string name)
    {
        _mapping.Add(type, name);
    }
}
