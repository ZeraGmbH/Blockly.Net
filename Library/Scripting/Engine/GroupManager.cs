using System.Text.Json;

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
    /// For nested groups the status to serialize to.
    /// </summary>
    public GroupStatus? _parentStatus = null;

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
    public IGroupManager CreateNested(string scriptId, string name)
    {
        var asGroup = new GroupStatus { Key = scriptId, Name = name };
        var child = new GroupManager { _parent = this, _parentStatus = asGroup };

        lock (_groups)
        {
            _scripts.Add(child);

            _groups.Add(asGroup);
        }

        return child;
    }

    /// <inheritdoc/>
    public void Finish(GroupResult result)
    {
        /* Get the active one and set the result - fire some exception if none is found. */
        lock (_groups)
            _active.Pop().SetResult(result);
    }

    /// <inheritdoc/>
    public bool Start(string id, string? name)
    {
        var group = new GroupStatus { Key = id, Name = name };

        lock (_groups)
        {
            /* Build a tree. */
            if (_active.TryPeek(out var parent))
                parent.Children.Add(group);
            else
                _groups.Add(group);

            _active.Push(group);
        }

        return true;
    }

    /// <inheritdoc/>
    public List<GroupStatus> Serialize()
    {
        lock (_groups)
        {
            /* Ask the whole tree to serialize itself. */
            foreach (var nested in _scripts)
                nested._parentStatus!.Children = nested.Serialize();

            /* Just report what we collected - make sure that we clone inside the lock. */
            return JsonSerializer.Deserialize<List<GroupStatus>>(JsonSerializer.Serialize(_groups, JsonUtils.JsonSettings), JsonUtils.JsonSettings)!;
        }
    }
}