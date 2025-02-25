

using System.Xml;
using BlocklyNet.Core.Blocks.Variables;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core;

/// <summary>
/// 
/// </summary>
public class XmlParser : Parser<XmlParser>
{
  /// <inheritdoc/>
  public override Workspace Parse(string xml, bool preserveWhitespace = false)
  {
    var xdoc = new XmlDocument { PreserveWhitespace = preserveWhitespace };

    xdoc.LoadXml(xml);

    var workspace = new Workspace();

    foreach (XmlNode node in xdoc.DocumentElement!.ChildNodes)
    {
      if (node.LocalName == "block" || node.LocalName == "shadow")
      {
        var block = ParseBlock(node, workspace);

        if (block != null) workspace.Blocks.Add(block);
      }
      else
      {
        // Fast-Solution :-)
        // Global variables should be parsed, else the assessment of any global variable in the script will cause an Application Exception("unexpected value type") exception. 

        // only global variables should be parsed in this context.  
        if (node.LocalName != "variables" || node.FirstChild == null ||
            string.IsNullOrWhiteSpace(node.FirstChild.InnerText))
          continue;

        foreach (XmlNode nodeChild in node.ChildNodes)
        {
          if (nodeChild.LocalName != "variable" || string.IsNullOrWhiteSpace(nodeChild.InnerText))
            continue;

          // Generate variable members    
          var block = new GlobalVariablesSet();

          var field = new Field { Name = "VAR", Value = nodeChild.InnerText, Type = nodeChild.GetAttribute("variabletype") };

          block.Fields.Add(field);

          var value = new Value { Name = "VALUE", Block = null };

          block.Values.Add(value);

          workspace.Blocks.Add(block);

          // Check for type
          var type = nodeChild.GetAttribute("type");

          if (!string.IsNullOrEmpty(type))
            workspace.VariableTypes[field.Value] = type;
        }
      }
    }

    return workspace;
  }

  private Block? ParseBlock(XmlNode node, Workspace workspace)
  {
    var type = node.GetAttribute("type");

    if (!blocks.ContainsKey(type))
      throw new ApplicationException($"block type not registered: '{type}'");

    var block = blocks[type]();

    workspace.ParsedBlocks.Add(block);

    block.Enabled = node.GetAttribute("disabled-reasons") == null;
    block.Id = node.GetAttribute("id");
    block.Type = type;

    foreach (XmlNode childNode in node.ChildNodes)
    {
      switch (childNode.LocalName)
      {
        case "mutation":
          ParseMutation(childNode, block);
          break;
        case "field":
          ParseField(childNode, block);
          break;
        case "value":
          ParseValue(childNode, block, workspace);
          break;
        case "statement":
          ParseStatement(childNode, block, workspace);
          break;
        case "comment":
          ParseComment(childNode, block);
          break;
        case "next":
          var nextBlock = ParseBlock(childNode.FirstChild!, workspace);

          if (nextBlock != null) block.Next = nextBlock;
          break;
        default:
          throw new ArgumentException($"unknown xml type: {childNode.LocalName}");
      }
    }
    return block;
  }

  private static void ParseField(XmlNode fieldNode, Block block)
  {
    var field = new Field
    {
      Name = fieldNode.GetAttribute("name"),
      Value = fieldNode.InnerText
    };
    block.Fields.Add(field);
  }

  private void ParseValue(XmlNode valueNode, Block block, Workspace workspace)
  {
    var childNode = valueNode.GetChild("block") ?? valueNode.GetChild("shadow");
    if (childNode == null)
      return;

    var childBlock = ParseBlock(childNode, workspace)!;

    var value = new Value
    {
      Name = valueNode.GetAttribute("name"),
      Block = childBlock
    };
    block.Values.Add(value);
  }

  private static void ParseComment(XmlNode commentNode, Block block)
  {
    block.Comments.Add(new Comment(commentNode.InnerText));
  }

  private void ParseStatement(XmlNode statementNode, Block block, Workspace workspace)
  {
    var childNode = statementNode.GetChild("block") ?? statementNode.GetChild("shadow");
    if (childNode == null)
      return;
    var childBlock = ParseBlock(childNode, workspace);

    var statement = new Statement
    {
      Name = statementNode.GetAttribute("name"),
      Block = childBlock
    };
    block.Statements.Add(statement);
  }

  private static void ParseMutation(XmlNode mutationNode, Block block)
  {
    foreach (XmlAttribute attribute in mutationNode.Attributes!)
    {
      block.Mutations.Add(new Mutation("mutation", attribute.Name, attribute.Value));
    }

    foreach (XmlNode node in mutationNode.ChildNodes)
    {
      foreach (XmlAttribute attribute in node.Attributes!)
      {
        block.Mutations.Add(new Mutation(node.Name, attribute.Name, attribute.Value));
      }
    }
  }

}

internal static class XmlParserExtensions
{
  public static XmlNode? GetChild(this XmlNode node, string name)
  {
    foreach (XmlNode childNode in node.ChildNodes)
    {
      if (childNode.LocalName == name)
        return childNode;
    }
    return null;
  }

  public static string GetAttribute(this XmlNode node, string name)
  {
    foreach (XmlAttribute attribute in node.Attributes!)
    {
      if (attribute.Name == name)
        return attribute.Value;
    }
    return null!;
  }
}