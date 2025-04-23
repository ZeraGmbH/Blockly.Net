

using BlocklyNet.Core.Model;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathOnList : Block
{
  private static readonly Random rnd = new Random();

  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var cvt = context.ServiceProvider.GetService<IDoubleExtractor>();

    var op = Fields["OP"];
    var list = await Values.EvaluateAsync<IEnumerable<object>>("LIST", context);

    var doubleList = list.Select(raw =>
    {
      if (cvt == null) return (double)raw;

      /* Run converter - report cast error on mismatch. */
      try
      {
        return cvt.GetNumber(raw);
      }
      catch (Exception)
      {
        return (double)raw;
      }
    }).ToArray();

    return op switch
    {
      "AVERAGE" => doubleList.Average(),
      "MAX" => doubleList.Max(),
      "MEDIAN" => Median(doubleList)!,
      "MIN" => doubleList.Min(),
      "MODE" => doubleList.Length != 0 ? doubleList.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key : null!,
      "RANDOM" => doubleList.Length != 0 ? doubleList[rnd.Next(doubleList.Length)] : null!,
      "STD_DEV" => throw new NotImplementedException($"OP {op} not implemented"),
      "SUM" => doubleList.Sum(),
      _ => throw new ApplicationException($"Unknown OP {op}"),
    };
  }

  private static object? Median(IEnumerable<double> values)
  {
    if (!values.Any()) return null;

    var sortedValues = values.OrderBy(x => x).ToArray();
    var mid = (sortedValues.Length - 1) / 2.0;

    return (sortedValues[(int)(mid + 0.0)] + sortedValues[(int)(mid + 0.5)]) / 2;
  }
}