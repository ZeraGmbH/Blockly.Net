using System.Text.Json;
using BlocklyNet;
using BlocklyNet.Scripting.Engine;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class GroupManagerTests
{
    public static GroupRepeat MakeRepeat(GroupStatus status, GroupRepeatType type)
    {
        var repeat = JsonSerializer.Deserialize<GroupRepeat>(JsonSerializer.Serialize(status, JsonUtils.JsonSettings), JsonUtils.JsonSettings)!;

        repeat.Repeat = type;

        return repeat;
    }

    private IGroupManager Manager = null!;

    [SetUp]
    public void Setup()
    {
        Manager = new GroupManager();
    }

    [Test]
    public void Empty_Group_Manager_Will_Serialize_To_Null()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Manager.Serialize(), Has.Count.EqualTo(0));
            Assert.That(Manager.CreateFlatResults(), Is.Null);
        });
    }

    [Test]
    public async Task Group_Manager_Can_Be_Reset_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        Manager.Reset(null);

        Assert.Multiple(() =>
        {
            Assert.That(Manager.Serialize(), Has.Count.EqualTo(0));
            Assert.That(Manager.CreateFlatResults(), Is.Null);
        });
    }

    [Test]
    public async Task Bad_EndGroup_Will_Throw_Exception_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        Assert.ThrowsAsync<InvalidOperationException>(() => Manager.FinishAsync(new() { Type = GroupResultType.Failed }));
    }

    [Test]
    public async Task Can_Manage_Three_Groups_In_Sequence_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Failed });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        var groups = Manager.Serialize();

        Assert.That(groups, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(groups[0].Key, Is.EqualTo("1"));
            Assert.That(groups[0].Name, Is.EqualTo("n1"));
            Assert.That(groups[0].Children, Has.Count.EqualTo(0));
            Assert.That(groups[0].GetResult()!.Type, Is.EqualTo(GroupResultType.Succeeded));
            Assert.That(groups[1].Key, Is.EqualTo("2"));
            Assert.That(groups[1].Name, Is.EqualTo("n2"));
            Assert.That(groups[1].Children, Has.Count.EqualTo(0));
            Assert.That(groups[1].GetResult()!.Type, Is.EqualTo(GroupResultType.Failed));
            Assert.That(groups[2].Key, Is.EqualTo("3"));
            Assert.That(groups[2].Name, Is.EqualTo("n3"));
            Assert.That(groups[2].Children, Has.Count.EqualTo(0));
            Assert.That(groups[2].GetResult()!.Type, Is.EqualTo(GroupResultType.Succeeded));
        });
    }

    [Test]
    public async Task Can_Manage_Three_NestedGroups_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        var groups = Manager.Serialize();

        Assert.That(groups, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(groups[0].Key, Is.EqualTo("1"));
            Assert.That(groups[0].Name, Is.EqualTo("n1"));
            Assert.That(groups[0].Children, Has.Count.EqualTo(1));
            Assert.That(groups[1].Key, Is.EqualTo("3"));
            Assert.That(groups[1].Name, Is.EqualTo("n3"));
            Assert.That(groups[1].Children, Has.Count.EqualTo(0));
        });

        Assert.Multiple(() =>
        {
            Assert.That(groups[0].Children[0].Key, Is.EqualTo("2"));
            Assert.That(groups[0].Children[0].Name, Is.EqualTo("n2"));
            Assert.That(groups[0].Children[0].Children, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public async Task Can_Have_Nested_Script_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        var nested = await Manager.CreateNestedAsync("ID", "NAME");

        await nested.StartAsync("2", "n2", null);
        await nested.FinishAsync(new() { Type = GroupResultType.Succeeded });

        await nested.StartAsync("3", "n3", null);
        await nested.FinishAsync(new() { Type = GroupResultType.Succeeded });

        Assert.That(await Manager.StartAsync("4", "n4", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        var groups = Manager.Serialize();

        Assert.That(groups, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(groups[0].Key, Is.EqualTo("1"));
            Assert.That(groups[0].Name, Is.EqualTo("n1"));
            Assert.That(groups[0].Children, Has.Count.EqualTo(0));
            Assert.That(groups[1].Key, Is.EqualTo("ID"));
            Assert.That(groups[1].Name, Is.EqualTo("NAME"));
            Assert.That(groups[1].Children, Has.Count.EqualTo(2));
            Assert.That(groups[2].Key, Is.EqualTo("4"));
            Assert.That(groups[2].Name, Is.EqualTo("n4"));
            Assert.That(groups[2].Children, Has.Count.EqualTo(0));
        });

        Assert.Multiple(() =>
        {
            Assert.That(groups[1].Children[0].Key, Is.EqualTo("2"));
            Assert.That(groups[1].Children[0].Name, Is.EqualTo("n2"));
            Assert.That(groups[1].Children[0].Children, Has.Count.EqualTo(0));
            Assert.That(groups[1].Children[1].Key, Is.EqualTo("3"));
            Assert.That(groups[1].Children[1].Name, Is.EqualTo("n3"));
            Assert.That(groups[1].Children[1].Children, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public async Task Can_Flatten_Result_To_Array_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var nested = await Manager.CreateNestedAsync("ID", "NAME");

        await nested.StartAsync("2", "n2", null);
        await nested.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 2 });

        await nested.StartAsync("3", "n3", null);
        await nested.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        Assert.That(await Manager.StartAsync("4", "n4", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 4 });

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(4));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(2));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
            Assert.That(((JsonElement)results[3]!).ToJsonScalar(), Is.EqualTo(4));
        });
    }

    [Test]
    public async Task Can_Retrieve_Status_On_Unfinished_Groups_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);

        var groups = Manager.Serialize();

        Assert.That(groups, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(groups[0].Key, Is.EqualTo("1"));
            Assert.That(groups[0].Name, Is.EqualTo("n1"));
            Assert.That(groups[0].Children, Has.Count.EqualTo(1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(groups[0].Children[0].Key, Is.EqualTo("2"));
            Assert.That(groups[0].Children[0].Name, Is.EqualTo("n2"));
            Assert.That(groups[0].Children[0].Children, Has.Count.EqualTo(0));
        });

        Assert.That(Manager.CreateFlatResults(), Has.Count.EqualTo(0));

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded });

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.Null);
    }

    [Test]
    public async Task Can_Restart_Sequence_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Failed, Result = 2 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            MakeRepeat(groups[1], GroupRepeatType.Again),
            MakeRepeat(groups[2], GroupRepeatType.Skip),
        ]);

        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Not.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Failed, Result = 4 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Not.Null);

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(4));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Can_Restart_Sequence_With_Nesting_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Failed, Result = 2 });

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            MakeRepeat(groups[1], GroupRepeatType.Skip),
        ]);

        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Not.Null);

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Not.Null);

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(2));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Can_Restart_Sequence_With_Nested_Skip_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Failed, Result = 2 });

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var repeat0 = MakeRepeat(groups[0], GroupRepeatType.Again);

        repeat0.Children[0].Repeat = GroupRepeatType.Skip;

        Manager.Reset([
            repeat0,
            MakeRepeat(groups[1], GroupRepeatType.Skip),
        ]);

        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Not.Null);

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Not.Null);

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(2));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Can_Restart_Sequence_With_Script_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var nested = await Manager.CreateNestedAsync("ID", "script");

        Assert.That(await nested.StartAsync("2", "n2", null), Is.Null);
        await nested.FinishAsync(new() { Type = GroupResultType.Failed, Result = 2 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var script = MakeRepeat(groups[1], GroupRepeatType.Unset);

        script.Children[0].Repeat = GroupRepeatType.Skip;

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            script,
            MakeRepeat(groups[2], GroupRepeatType.Skip),
        ]);

        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Not.Null);

        nested = await Manager.CreateNestedAsync("ID", "script");

        Assert.That(await nested.StartAsync("2", "n2", null), Is.Not.Null);

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Not.Null);

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(2));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Can_Restart_Sequence_With_Script_Nested_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var nested = await Manager.CreateNestedAsync("ID", "script");

        Assert.That(await nested.StartAsync("2", "n2", null), Is.Null);
        await nested.FinishAsync(new() { Type = GroupResultType.Failed, Result = 2 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var script = MakeRepeat(groups[1], GroupRepeatType.Unset);

        script.Children[0].Repeat = GroupRepeatType.Again;

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            script,
            MakeRepeat(groups[2], GroupRepeatType.Skip),
        ]);

        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Not.Null);

        nested = await Manager.CreateNestedAsync("ID", "script");

        Assert.That(await nested.StartAsync("2", "n2", null), Is.Null);
        await nested.FinishAsync(new() { Type = GroupResultType.Failed, Result = 4 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Not.Null);

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(4));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
        });
    }

    [Test]
    public async Task Can_Merge_Repeat_Information_Async()
    {
        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Failed, Result = 2 });

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(await Manager.StartAsync("3", "n3", null), Is.Null);
        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var repeat0 = MakeRepeat(groups[0], GroupRepeatType.Again);

        repeat0.Children[0].Repeat = GroupRepeatType.Skip;

        Manager.Reset([
            repeat0,
            MakeRepeat(groups[1], GroupRepeatType.Skip),
        ]);

        Assert.That(await Manager.StartAsync("1", "n1", null), Is.Null);

        Assert.That(await Manager.StartAsync("2", "n2", null), Is.Not.Null);

        await Manager.FinishAsync(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(((JsonElement)results[0]!).ToJsonScalar(), Is.EqualTo(2));
            Assert.That(((JsonElement)results[1]!).ToJsonScalar(), Is.EqualTo(1));
            Assert.That(((JsonElement)results[2]!).ToJsonScalar(), Is.EqualTo(3));
        });
    }
}