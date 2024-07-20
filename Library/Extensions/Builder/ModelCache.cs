namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Helper class to manage the block building process
/// </summary>
public class ModelCache
{
    /// <summary>
    /// Wrapper on a function to make sure that it is executed only once.
    /// </summary>
    /// <param name="factory">Factory method to use.</param>
    private class Factory(Action? factory)
    {
        /// <summary>
        /// The factory method - will be reset after first call.
        /// </summary>
        private Action? _factory = factory;

        /// <summary>
        /// Execute the method once.
        /// </summary>
        public void ExecuteOnce() => Interlocked.Exchange(ref _factory, null)?.Invoke();
    }

    /// <summary>
    /// A single type registraion.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="factory"></param>
    private class TypeInfo(string name, Factory factory)
    {
        /// <summary>
        /// Blockly type name of the model.
        /// </summary>
        public readonly string Name = name;

        /// <summary>
        /// Factory to call to create the block definition.
        /// </summary>
        public readonly Factory Factory = factory;
    }

    /// <summary>
    /// All defined blocks.
    /// </summary>
    private readonly Dictionary<Type, TypeInfo> _mapping = [];

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
    public string this[Type type]
    {
        get
        {
            /* Create the definition once. */
            var info = _mapping[type];

            info.Factory.ExecuteOnce();

            return info.Name;
        }
    }

    /// <summary>
    /// Register an additional block type.
    /// </summary>
    /// <param name="name">Blockly type name for the model.</param>
    /// <param name="factory">Method to create the block.</param>
    /// <typeparam name="T">Model type.</typeparam>
    public void Add<T>(string name, Action? factory = null) => Add(typeof(T), name, factory);

    /// <summary>
    /// Register an additional block type.
    /// </summary>
    /// <param name="type">Model type.</param>
    /// <param name="factory">Method to create the block.</param>
    /// <param name="name">Blockly type name for the model.</param>
    public void Add(Type type, string name, Action? factory = null) => _mapping.Add(type, new(name, new Factory(factory)));

    /// <summary>
    /// Create all blocky definitions.
    /// </summary>
    public void CreateDefinitions() => _mapping.Values.ToList().ForEach(i => i.Factory.ExecuteOnce());
}
