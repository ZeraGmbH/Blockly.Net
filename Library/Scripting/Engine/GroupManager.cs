using System.Text.Json;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Supports execution groups.
/// </summary>
public class GroupManager : IGroupManager
{
    /// <summary>
    /// Helper to access previous execution data.
    /// </summary>
    /// <param name="_groups">The list of known group status from a previous run.</param>
    /// <param name="parent">The helper of the parent group information.</param>
    private class Previous(List<GroupRepeat> _groups, Previous? parent = null)
    {
        /// <summary>
        /// Initial data.
        /// </summary>
        public readonly List<GroupRepeat> Groups = _groups;

        /// <summary>
        /// Current group data to inspect.
        /// </summary>
        private int _index = 0;

        /// <summary>
        /// Report the current group and advance to the next.
        /// </summary>
        /// <returns>Unset if no data is available.</returns>
        public GroupRepeat? GetAndAdvance() => _index >= Groups.Count ? null : Groups[_index++];

        /// <summary>
        /// Create a new helper based on the current group.
        /// </summary>
        /// <returns>New helper on the nested group data.</returns>
        public Previous? Push() => _index >= Groups.Count ? null : new(Groups[_index].Children, this);

        /// <summary>
        /// Go back to the parent helper.
        /// </summary>
        /// <returns>Unset if we are the root.</returns>
        public Previous? Pop() => parent;
    }

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

    /// <summary>
    /// Information on the results of a previous execution.
    /// </summary>
    private Previous? _previous;

    /// <inheritdoc/>
    public void Reset(IEnumerable<GroupRepeat>? previous)
    {
        lock (_groups)
        {
            _active.Clear();
            _groups.Clear();
            _scripts.Clear();

            _previous = new([.. previous ?? []]);
        }
    }

    /// <inheritdoc/>
    public IGroupManager CreateNested(string scriptId, string name) => CreateGroup(scriptId, name, true).Item2;

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
            /* New previous group information helper. */
            var previous = _previous?.Push();

            /* Get current information for inspection. */
            var current = _previous?.GetAndAdvance();

            if (current == null || current.IsScript != nested || current.Key != id)
                _previous = null;

            /* Create a nested group management instance. */
            if (nested)
            {
                manager = new GroupManager { _parentStatus = group, _previous = previous };

                _scripts.Add(manager);
            }

            /* Build a tree. */
            if (_active.TryPeek(out var parent))
                parent.Children.Add(group);
            else
                _groups.Add(group);

            /* Do a real start requiring some finish later on - not for nested scripts. */
            if (!nested)
            {
                /* Check for auto-finish. */
                var result = current?.GetResult();

                if (result != null && current!.Repeat == GroupRepeatType.Skip)
                {
                    /* Simulate execution. */
                    group.SetResult(new() { Type = result.Type, Result = result.Result });

                    group.Children.AddRange(current.Children.Select(g => g.ToStatus()));

                    /* Caller can abort nested proceessing. */
                    return Tuple.Create(false, (IGroupManager)manager);
                }

                /* Full process. */
                if (_previous != null) _previous = previous;

                _active.Push(group);
            }
        }

        return Tuple.Create(true, (IGroupManager)manager);
    }

    /// <inheritdoc/>
    public void Finish(GroupResult result)
    {
        /* Get the active one and set the result - fire some exception if none is found. */
        lock (_groups)
        {
            _active.Pop().SetResult(new() { Type = result.Type, Result = result.Result });

            _previous = _previous?.Pop();
        }
    }

    /// <summary>
    /// Expand a list of group execution information with previously known data.
    /// </summary>
    /// <param name="groups">List of groups which can be update.</param>
    /// <param name="previous">All previous group informations.</param>
    private static void MergePrevious(List<GroupStatus> groups, List<GroupRepeat> previous)
    {
        for (var i = 0; i < groups.Count; i++)
            if (i < previous.Count)
                MergePrevious(groups[i].Children, previous[i].Children);

        for (var i = groups.Count; i < previous.Count; i++)
            groups.Add(previous[i].ToStatus());
    }

    /// <inheritdoc/>
    public List<GroupStatus> Serialize(bool includeRepeat = false)
    {
        lock (_groups)
        {
            /* Ask the whole tree to serialize itself. */
            foreach (var nested in _scripts)
                nested._parentStatus!.Children = nested.Serialize();

            /* Just report what we collected - make sure that we clone inside the lock. */
            var groups = JsonSerializer.Deserialize<List<GroupStatus>>(JsonSerializer.Serialize(_groups, JsonUtils.JsonSettings), JsonUtils.JsonSettings)!;

            if (includeRepeat)
                MergePrevious(groups, _previous?.Groups ?? []);

            return groups;
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
        Serialize(true).ForEach(copyToResults);

        return results;
    }
}