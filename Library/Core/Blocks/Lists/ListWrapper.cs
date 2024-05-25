namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// Helper to allow IList access to a generic list -
/// in contrast to IEnumerable there is no non-generic
/// interface IList.
/// </summary>
public class ListWrapper
{
    private readonly object _list;

    private readonly Type _type;

    /// <summary>
    /// Initialze a new wrapper.
    /// </summary>
    /// <param name="list">List-like object.</param>
    public ListWrapper(object? list)
    {
        _list = list!;
        _type = _list.GetType();
    }

    /// <summary>
    /// Report the number of items in the list.
    /// </summary>
    public int Count => (int)_type.GetProperty("Count")!.GetValue(_list)!;

    /// <summary>
    /// Remove a single element from the list.
    /// </summary>
    /// <param name="index">Index to remove.</param>
    public void RemoveAt(int index) => _type.GetMethod("RemoveAt")!.Invoke(_list, [index]);

    /// <summary>
    /// Insert an element in the list.
    /// </summary>
    /// <param name="index">Index for the new element.</param>
    /// <param name="value">Value of the new element.</param>
    public void InsertAt(int index, object? value) => _type.GetMethod("Insert")!.Invoke(_list, [index, value]);

    /// <summary>
    /// Access an element by index.
    /// </summary>
    /// <param name="index">Index of the element.</param>
    /// <returns>Value of the element - in case of reading from the list.</returns>
    public object? this[int index]
    {
        get
        {
            return _type.IsArray
                ? ((Array)_list).GetValue(index)
                : _type.GetProperty("Item")!.GetValue(_list, [index]);
        }
        set
        {
            if (_type.IsArray)
                ((Array)_list).SetValue(value, index);
            else
                _type.GetProperty("Item")!.SetValue(_list, value, [index]);
        }
    }
}
