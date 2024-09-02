

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextAppend : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var variables = context.Variables;

    var variableName = Fields["VAR"];
    var textToAppend = (await Values.EvaluateAsync("TEXT", context) ?? "").ToString();

    if (!variables.ContainsKey(variableName))
      variables.Add(variableName, "");

    var value = variables[variableName]!.ToString();

    variables[variableName] = value + textToAppend;

    return await base.EvaluateAsync(context);
  }
}