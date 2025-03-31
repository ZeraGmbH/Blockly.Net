

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Variables;

/// <summary>
/// 
/// </summary>
public class VariablesSet : Block
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="context"></param>
  /// <param name="name"></param>
  /// <param name="value"></param>
  public static void Set(Context context, string name, object? value)
  {
    var variables = context.Variables;

    // Fast-Solution
    if (variables.ContainsKey(name))
      variables[name] = value!;
    else
    {
      var rootContext = context.GetRootContext();

      if (rootContext.Variables.ContainsKey(name))
        rootContext.Variables[name] = value!;
      else
        variables.Add(name, value!);
    }
  }

  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    Set(context, Fields["VAR"], await Values.EvaluateAsync("VALUE", context));

    return await base.EvaluateAsync(context);
  }
}