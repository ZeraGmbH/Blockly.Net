using System.Text.Json;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Supports execution groups.
/// </summary>
public class GroupManager : IGroupManager
{
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
    public void Reset(IEnumerable<GroupRepeat>? previous)
    {
        lock (_groups)
        {
            _active.Clear();
            _groups.Clear();
            _scripts.Clear();
        }
    }

    /// <inheritdoc/>
    public IGroupManager CreateNested(string scriptId, string name) => CreateGroup(scriptId, name, true).Item2;

    /// <inheritdoc/>
    public void Finish(GroupResult result)
    {
        /* Get the active one and set the result - fire some exception if none is found. */
        lock (_groups)
            _active.Pop().SetResult(new() { Type = result.Type, Result = result.Result });
    }

    /// <inheritdoc/>
    public bool Start(string id, string? name) => CreateGroup(id, name, false).Item1;

    private Tuple<bool, IGroupManager> CreateGroup(string id, string? name, bool nested)
    {
        /* Maybe a nested group manager must be created. */
        GroupManager manager = null!;

        /* The new group. */
        var group = new GroupStatus { Key = id, Name = name, IsScript = nested };

        lock (_groups)
        {
            /* Create a nested group management instance. */
            if (nested)
            {
                manager = new GroupManager { _parentStatus = group };

                _scripts.Add(manager);
            }

            /* Build a tree. */
            if (_active.TryPeek(out var parent))
                parent.Children.Add(group);
            else
                _groups.Add(group);

            /* Do a real start requiring some finish later on - not for nested scripts. */
            if (!nested)
                _active.Push(group);
        }

        return Tuple.Create(true, (IGroupManager)manager);
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

    /// <inheritdoc/>
    public List<object?>? CreateFlatResults()
    {
        /* Execution groups are not used. */
        lock (_groups)
            if (_groups.Count < 1)
                return null;

        /* Construct list - actually it may be empty but this is different from not using groups at all. */
        var results = new List<object?>();

        /* Make sure nested scripts have anything expanded. */
        Action<GroupStatus> copyToResults = null!;

        copyToResults = (group) =>
        {
            /* Children first. */
            group.Children.ForEach(copyToResults);

            /* Self last. */
            var result = group.GetResult();

            /* Only add results for finished groups. */
            if (result != null && result.Type != GroupResultType.Invalid)
                results.Add(result.Result);
        };

        /* Process all top level groups. */
        Serialize().ForEach(copyToResults);

        return results;
    }
}