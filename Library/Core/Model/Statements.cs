namespace BlocklyNet.Core.Model;

/// <summary>
/// All statements of a block.
/// </summary>
public class Statements : Entities<Statement>
{
    /// <summary>
    /// Get the key of a statement.
    /// </summary>
    /// <param name="statement">The related statement.</param>
    /// <returns>The name of the statement.</returns>
    protected override string GetKey(Statement statement) => statement.Name;

    /// <summary>
    /// Find a statement.
    /// </summary>
    /// <param name="name">Name of the statement.</param>
    /// <returns>The statement.</returns>
    /// <exception cref="KeyNotFoundException">There is no such statement.</exception>
    public Statement this[string name] => Get(name);
}
