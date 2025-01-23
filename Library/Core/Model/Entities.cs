using System.Collections;

namespace BlocklyNet.Core.Model;

/// <summary>
/// Similiar to a keyed collection but with the special
/// notion of the first item for any given key.
/// </summary>
/// <typeparam name="T">Type of items managed.</typeparam>
public abstract class Entities<T> : IEnumerable<T>
{
    /// <summary>
    /// Retrieve the key of an item.
    /// </summary>
    /// <param name="item">Some item.</param>
    /// <returns>The key to use for this item.</returns>
    protected abstract string GetKey(T item);

    private readonly List<T> _itemList = [];

    private readonly Dictionary<string, T> _firstItemMap = [];

    /// <summary>
    /// Add a brand new item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        /* Always add tp the list. */
        _itemList.Add(item);

        /* Remember the first item for any given key for fast access. */
        _firstItemMap.TryAdd(GetKey(item), item);
    }

    /// <summary>
    /// Retrieve the first item for a given key.
    /// </summary>
    /// <param name="key">Key of the item.</param>
    /// <returns>The item.</returns>
    /// <exception cref="KeyNotFoundException">There is no such item.</exception>
    public T Get(string key) => _firstItemMap[key];

    /// <summary>
    /// Retrieve the first item for a given key.
    /// </summary>
    /// <param name="key">Key of the item.</param>
    /// <returns>The item or null if the item is not known.</returns>
    public T? TryGet(string key) => _firstItemMap.TryGetValue(key, out var value) ? value : default;

    /// <summary>
    /// Check if an item exists.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <returns>Set if the item is known.</returns>
    public bool Has(string key) => _firstItemMap.ContainsKey(key);

    /// <summary>
    /// Create an enumerator on all items.
    /// </summary>
    /// <returns>All items added to this manager.</returns>
    public IEnumerator<T> GetEnumerator() => _itemList.GetEnumerator();

    /// <summary>
    /// Create an enumerator on all items.
    /// </summary>
    /// <returns>All items added to this manager.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
