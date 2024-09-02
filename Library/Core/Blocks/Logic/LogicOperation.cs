

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicOperation : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var a = await Values.EvaluateAsync<bool?>("A", context) ?? false;
    var b = await Values.EvaluateAsync<bool?>("B", context) ?? false;

    var op = Fields["OP"];

    switch (op)
    {
      case "AND": return a && b;
      case "OR": return a || b;
      default: throw new ApplicationException($"Unknown OP {op}");
    }

  }
}