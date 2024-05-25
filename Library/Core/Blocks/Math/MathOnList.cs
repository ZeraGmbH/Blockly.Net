

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

public class MathOnList : Block
{
  private static readonly Random rnd = new Random();
  public override async Task<object?> Evaluate(Context context)
  {
    var op = Fields["OP"];
    var list = await Values.Evaluate<IEnumerable<object>>("LIST", context);

    var doubleList = list.Select(x => (double)x).ToArray();

    switch (op)
    {
      case "SUM": return doubleList.Sum();
      case "MIN": return doubleList.Min();
      case "MAX": return doubleList.Max();
      case "AVERAGE": return doubleList.Average();
      case "MEDIAN": return Median(doubleList)!;
      case "RANDOM": return doubleList.Length != 0 ? doubleList[rnd.Next(doubleList.Count())] : null!;
      case "MODE": return doubleList.Length != 0 ? doubleList.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key : null!;

      case "STD_DEV":
        throw new NotImplementedException($"OP {op} not implemented");

      default: throw new ApplicationException($"Unknown OP {op}");
    }
  }

  private static object? Median(IEnumerable<double> values)
  {
    if (!values.Any())
      return null;
    var sortedValues = values.OrderBy(x => x).ToArray();
    var mid = (sortedValues.Length - 1) / 2.0;
    return (sortedValues[(int)(mid)] + sortedValues[(int)(mid + 0.5)]) / 2;
  }
}