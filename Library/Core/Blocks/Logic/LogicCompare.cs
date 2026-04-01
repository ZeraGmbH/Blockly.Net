

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicCompare : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var opValue = Fields["OP"];

    var a = await Values.EvaluateAsync<object>("A", context);
    var b = await Values.EvaluateAsync<object>("B", context);

    if (a == null || b == null) return Compare(opValue, a == null, b == null);

    var tryInt = TryConvertToDoubleValues(a, b, context);
    if (tryInt.canConvert)
      return Compare(opValue, tryInt.aValue, tryInt.bValue);

    var tryString = TryConvertValues<string>(a, b);
    if (tryString.canConvert)
      return Compare(opValue, tryString.aValue, tryString.bValue);

    var tryBool = TryConvertValues<bool>(a, b);
    if (tryBool.canConvert)
      return Compare(opValue, tryBool.aValue, tryBool.bValue);

    if (a is IComparable genericA && b is IComparable genericB)
      return CompareGeneric(opValue, genericA, genericB);

    throw new ApplicationException("unexpected value type");
  }

  private static bool Compare(string op, string a, string b)
  {
    switch (op)
    {
      case "EQ":
        return a == b;
      case "NEQ":
        return a != b;
      case "LT":
        return string.CompareOrdinal(a, b) < 0;
      case "LTE":
        return string.CompareOrdinal(a, b) <= 0;
      case "GT":
        return string.CompareOrdinal(a, b) > 0;
      case "GTE":
        return string.CompareOrdinal(a, b) >= 0;
      default:
        throw new ApplicationException($"Unknown OP {op}");
    }
  }

  private static bool CompareGeneric(string op, IComparable a, IComparable b)
  {
    var delta = a.CompareTo(b);

    return op switch
    {
      "EQ" => delta == 0,
      "NEQ" => delta != 0,
      "LT" => delta < 0,
      "LTE" => delta <= 0,
      "GT" => delta > 0,
      "GTE" => delta >= 0,
      _ => throw new ApplicationException($"Unknown OP {op}"),
    };
  }

  private static bool Compare(string op, double a, double b)
  {
    switch (op)
    {
      case "EQ":
        return a.Equals(b);
      case "NEQ":
        return !a.Equals(b);
      case "LT":
        return a < b;
      case "LTE":
        return a <= b;
      case "GT":
        return a > b;
      case "GTE":
        return a >= b;
      default:
        throw new ApplicationException($"Unknown OP {op}");
    }
  }

  private static bool Compare(string op, bool a, bool b)
  {
    switch (op)
    {
      case "EQ":
        return a == b;
      case "NEQ":
        return a != b;
      case "LT":
        return Convert.ToByte(a) < Convert.ToByte(b);
      case "LTE":
        return Convert.ToByte(a) <= Convert.ToByte(b);
      case "GT":
        return Convert.ToByte(a) > Convert.ToByte(b);
      case "GTE":
        return Convert.ToByte(a) >= Convert.ToByte(b);
      default:
        throw new ApplicationException($"Unknown OP {op}");
    }
  }

  private static (bool canConvert, T aValue, T bValue) TryConvertValues<T>(object a, object b)
  {
    T aResult;
    if (a?.GetType() == typeof(T))
      aResult = (T)Convert.ChangeType(a, typeof(T));
    else
      return (false, default!, default!);

    T bResult;
    if (b?.GetType() == typeof(T))
      bResult = (T)Convert.ChangeType(b, typeof(T));
    else
      return (false, default!, default!);

    return (true, aResult, bResult);
  }

  private static (bool canConvert, double aValue, double bValue) TryConvertToDoubleValues(object a, object b, Context context)
  {
    if (Values.TryConvertToDouble(a, context, out var aResult) && Values.TryConvertToDouble(b, context, out var bResult))
      return (true, aResult, bResult);

    return (false, default, default);
  }
}