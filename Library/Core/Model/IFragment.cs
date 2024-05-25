namespace BlocklyNet.Core.Model;

/// <summary>
/// Some excutable entity.
/// </summary>
public interface IFragment
{
    /// <summary>
    /// Execute the entity and report the result.
    /// </summary>
    /// <param name="context">Current exewcution context.</param>
    /// <returns>Result of the execution.</returns>
    Task<object?> Evaluate(Context context);
}

