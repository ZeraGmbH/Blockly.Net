using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// Block defining a function. */
/// </summary>
public class ProceduresDef : Block
{
  /// <summary>
  /// Analyse the definition.
  /// </summary>
  /// <param name="context">Current execution context.</param>
  /// <returns>Always null.</returns>
  public override Task<object?> EvaluateAsync(Context context)
  {
    /* Retrieve the name and the content of the function. */
    var name = Fields["NAME"];
    var statement = Statements.TryGet("STACK");

    /* Without a name a function can not be used. */
    if (string.IsNullOrWhiteSpace(name))
      return Task.FromResult((object?)null);

    /* Create a dummy statement if body is empty. */
    statement ??= new Statement { Block = null, Name = "STACK" };

    /* See if the function will returen something. */
    var ret = Values.TryGet("RETURN");

    if (ret != null)
    {
      var valueBlock = new ValueBlock(ret);

      /* Put it at the end of the code of the function. */
      if (statement.Block == null)
        statement.Block = valueBlock;
      else
        FindEndOfChain(statement.Block).Next = valueBlock;
    }

    /* Register or replace the function - last definition wins. */
    context.Functions[name] = statement;

    /* Nothign to return. */
    return Task.FromResult((object?)null);
  }

  /// <summary>
  /// Find the last block in a chain.
  /// </summary>
  /// <param name="block">Some block.</param>
  /// <returns>The latest block in the chain.</returns>
  private static Block FindEndOfChain(Block block)
  {
    for (; ; block = block.Next)
      if (block.Next == null)
        return block;
  }

  /// <summary>
  /// Block calculating a value.
  /// </summary>
  /// <param name="value">The value.</param>
  private class ValueBlock(Value value) : Block
  {
    /// <summary>
    /// Report the value definition itsself.
    /// </summary>
    private readonly Value value = value;

    /// <summary>
    /// Get the current value.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<object?> EvaluateAsync(Context context) => value.EvaluateAsync(context);
  }
}