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
    public void Group_Manager_Can_Be_Reset()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        Manager.Reset(null);

        Assert.Multiple(() =>
        {
            Assert.That(Manager.Serialize(), Has.Count.EqualTo(0));
            Assert.That(Manager.CreateFlatResults(), Is.Null);
        });
    }

    [Test]
    public void Bad_EndGroup_Will_Throw_Exception()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        Assert.Throws<InvalidOperationException>(() => Manager.Finish(new() { Type = GroupResultType.Failed }));
    }

    [Test]
    public void Can_Manage_Three_Groups_In_Sequence()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Failed });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

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
    public void Can_Manage_Three_NestedGroups()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

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
    public void Can_Have_Nested_Script()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        var nested = Manager.CreateNested("ID", "NAME");

        nested.Start("2", "n2", null);
        nested.Finish(new() { Type = GroupResultType.Succeeded });

        nested.Start("3", "n3", null);
        nested.Finish(new() { Type = GroupResultType.Succeeded });

        Assert.That(Manager.Start("4", "n4", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded });

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
    public void Can_Flatten_Result_To_Array()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var nested = Manager.CreateNested("ID", "NAME");

        nested.Start("2", "n2", null);
        nested.Finish(new() { Type = GroupResultType.Succeeded, Result = 2 });

        nested.Start("3", "n3", null);
        nested.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        Assert.That(Manager.Start("4", "n4", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 4 });

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
    public void Can_Retrieve_Status_On_Unfinished_Groups()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.True);

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

        Manager.Finish(new() { Type = GroupResultType.Succeeded });

        var results = Manager.CreateFlatResults();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.Null);
    }

    [Test]
    public void Can_Restart_Sequence()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Failed, Result = 2 });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            MakeRepeat(groups[1], GroupRepeatType.Again),
            MakeRepeat(groups[2], GroupRepeatType.Skip),
        ]);

        Assert.That(Manager.Start("1", "n1", null), Is.False);

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Failed, Result = 4 });

        Assert.That(Manager.Start("3", "n3", null), Is.False);

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
    public void Can_Restart_Sequence_With_Nesting()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Failed, Result = 2 });

        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            MakeRepeat(groups[1], GroupRepeatType.Skip),
        ]);

        Assert.That(Manager.Start("1", "n1", null), Is.False);

        Assert.That(Manager.Start("3", "n3", null), Is.False);

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
    public void Can_Restart_Sequence_With_Nested_Skip()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Failed, Result = 2 });

        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var repeat0 = MakeRepeat(groups[0], GroupRepeatType.Again);

        repeat0.Children[0].Repeat = GroupRepeatType.Skip;

        Manager.Reset([
            repeat0,
            MakeRepeat(groups[1], GroupRepeatType.Skip),
        ]);

        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.False);

        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(Manager.Start("3", "n3", null), Is.False);

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
    public void Can_Restart_Sequence_With_Script()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var nested = Manager.CreateNested("ID", "script");

        Assert.That(nested.Start("2", "n2", null), Is.True);
        nested.Finish(new() { Type = GroupResultType.Failed, Result = 2 });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var script = MakeRepeat(groups[1], GroupRepeatType.Unset);

        script.Children[0].Repeat = GroupRepeatType.Skip;

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            script,
            MakeRepeat(groups[2], GroupRepeatType.Skip),
        ]);

        Assert.That(Manager.Start("1", "n1", null), Is.False);

        nested = Manager.CreateNested("ID", "script");

        Assert.That(nested.Start("2", "n2", null), Is.False);

        Assert.That(Manager.Start("3", "n3", null), Is.False);

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
    public void Can_Restart_Sequence_With_Script_Nested()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        var nested = Manager.CreateNested("ID", "script");

        Assert.That(nested.Start("2", "n2", null), Is.True);
        nested.Finish(new() { Type = GroupResultType.Failed, Result = 2 });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var script = MakeRepeat(groups[1], GroupRepeatType.Unset);

        script.Children[0].Repeat = GroupRepeatType.Again;

        Manager.Reset([
            MakeRepeat(groups[0], GroupRepeatType.Skip),
            script,
            MakeRepeat(groups[2], GroupRepeatType.Skip),
        ]);

        Assert.That(Manager.Start("1", "n1", null), Is.False);

        nested = Manager.CreateNested("ID", "script");

        Assert.That(nested.Start("2", "n2", null), Is.True);
        nested.Finish(new() { Type = GroupResultType.Failed, Result = 4 });

        Assert.That(Manager.Start("3", "n3", null), Is.False);

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
    public void Can_Merge_Repeat_Information()
    {
        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Failed, Result = 2 });

        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

        Assert.That(Manager.Start("3", "n3", null), Is.True);
        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 3 });

        var groups = Manager.Serialize();

        var repeat0 = MakeRepeat(groups[0], GroupRepeatType.Again);

        repeat0.Children[0].Repeat = GroupRepeatType.Skip;

        Manager.Reset([
            repeat0,
            MakeRepeat(groups[1], GroupRepeatType.Skip),
        ]);

        Assert.That(Manager.Start("1", "n1", null), Is.True);

        Assert.That(Manager.Start("2", "n2", null), Is.False);

        Manager.Finish(new() { Type = GroupResultType.Succeeded, Result = 1 });

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