

using BlocklyNet.Core.Blocks.Variables;
using Newtonsoft.Json.Linq;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core;

public class JsonParser : Parser<JsonParser>
{
  private readonly Dictionary<string, string> _variables = [];

  public override Workspace Parse(string json, bool preserveWhitespace = false)
  {
    var workspace = new Workspace();
    var jdoc = JObject.Parse(json);

    ParseVariables(jdoc, workspace);
    ParseBlocks(jdoc, workspace);

    return workspace;
  }

  private void ParseVariables(JToken jdoc, Workspace workspace)
  {
    var variables = jdoc["variables"];

    if (variables == null)
      return;

    foreach (var variable in variables)
    {
      var id = (string)variable["id"]!;
      var name = (string)variable["name"]!;

      _variables[id] = name;

      GlobalVariablesSet setVariable = new();

      setVariable.Fields.Add(new() { Name = "VAR", Value = name });
      setVariable.Values.Add(new() { Name = "VALUE", Block = null });

      workspace.Blocks.Add(setVariable);
    }
  }

  private void ParseBlocks(JToken jdoc, Workspace workspace)
  {
    var blocks = jdoc["blocks"]?["blocks"];

    if (blocks == null)
      return;

    foreach (var jblock in blocks)
    {
      var block = ParseBlock(jblock);

      if (block != null)
        workspace.Blocks.Add(block);
    }
  }

  private Block? ParseBlock(JToken jblock)
  {
    if (((bool?)jblock["disabled"]).GetValueOrDefault() == true)
      return null;

    var type = (string)jblock["type"]!;

    if (!blocks.TryGetValue(type, out var factory))
      throw new ApplicationException($"block type not registered: '{type}'");

    var block = factory();

    block.Type = type;
    block.Id = (string)jblock["id"]!;

    var fields = jblock["fields"];

    if (fields != null)
      foreach (var field in fields)
        ParseField(field, block);

    var inputs = jblock["inputs"];

    if (inputs != null)
      foreach (var input in inputs)
        ParseInput(input, block);

    var icons = jblock["icons"];

    if (icons != null)
      foreach (var icon in icons)
        ParseComment(icon, block);

    var extra = jblock["extraState"];

    if (extra != null)
      ParseMutation(extra, block);

    var next = jblock["next"];

    if (next != null)
      block.Next = ParseBlock(next["block"]!);

    return block;
  }

  private void ParseField(JToken jfield, Block block)
  {
    var first = jfield.First!;

    block.Fields.Add(new() { Name = jfield.Path.Split(".")[^1], Value = first.HasValues ? _variables[(string)first["id"]!] : first.Value<string>()! });
  }

  private void ParseInput(JToken jvalue, Block block)
  {
    var jBlock = jvalue.First!["block"] ?? jvalue.First["shadow"];

    if (jBlock == null)
      return;

    var key = jvalue.Path.Split(".")[^1];
    var inputBlock = ParseBlock(jBlock)!;

    block.Values.Add(new() { Name = key, Block = inputBlock });
    block.Statements.Add(new() { Name = key, Block = inputBlock });
  }

  private static void ParseComment(JToken jicon, Block block)
  {
    var key = jicon.Path.Split(".")[^1];

    if (key != "comment")
      return;

    block.Comments.Add(new Comment((string)jicon.First!["text"]!));
  }

  private void ParseMutation(JToken jextra, Block block)
  {
    if (!jextra.HasValues)
      return;

    var name = (string?)jextra["name"];

    if (name != null)
      block.Mutations.Add(new Mutation("mutation", "name", name));

    var hasElse = (string?)jextra["hasElse"];

    if (hasElse != null)
      block.Mutations.Add(new Mutation("mutation", "else", "1"));

    var elseIfCount = (string?)jextra["elseIfCount"];

    if (elseIfCount != null)
      block.Mutations.Add(new Mutation("mutation", "elseIfCount", elseIfCount));

    var itemCount = (string?)jextra["itemCount"];

    if (itemCount != null)
      block.Mutations.Add(new Mutation("mutation", "items", itemCount));

    var hasStatements = (bool?)jextra["hasStatements"];

    if (hasStatements.HasValue)
      block.Mutations.Add(new Mutation("mutation", "statements", hasStatements.ToString()!.ToLower()));

    var args = jextra["params"];

    if (args != null)
      foreach (var arg in args)
        block.Mutations.Add(new Mutation("arg", "name", arg.HasValues ? _variables[(string)arg["id"]!]! : arg.Value<string>()!));
  }
}
