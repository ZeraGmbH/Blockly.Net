namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Supports execution groups.
/// </summary>
public class GroupManager : IGroupManager
{
    /// <summary>
    /// The parent group manager if we are nested.
    /// </summary>
    /*private*/
    public GroupManager? _parent = null;

    /// <summary>
    /// All known groups.
    /// </summary>
    private readonly List<GroupStatus> _groups = [];

    /// <summary>
    /// Group hierarchy of unfinished groups.
    /// </summary>
    private readonly Stack<GroupStatus> _active = [];

    /// <summary>
    /// All directly nested execution groups related to single scripts.
    /// </summary>
    private readonly List<GroupManager> _scripts = [];

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_groups)
        {
            _active.Clear();
            _scripts.Clear();
            _groups.Clear();
        }
    }

    /// <inheritdoc/>
    public IGroupManager CreateNested()
    {
        var child = new GroupManager { _parent = this };

        lock (_groups)
            _scripts.Add(child);

        return child;
    }

    /// <inheritdoc/>
    public void Finish(object? result)
    {
        lock (_groups)
        {
            /* Get the active one - fire some exception if none is found. */
            var active = _active.Pop();

            /* Attach result. */
            active.Result = result;

            /* Build a tree. */
            if (_active.TryPeek(out var parent))
                parent.Children.Add(active);
            else
                _groups.Add(active);
        }
    }

    /// <inheritdoc/>
    public void Start(string id)
    {
        lock (_groups)
            _active.Push(new() { Key = id });
    }

    /// <inheritdoc/>
    public GroupStatus[] Serialize()
    {
        lock (_groups)
        {
            if (_active.Count < 1) return [.. _groups];

            throw new InvalidOperationException("invalid execution group nesting");
        }
    }
}