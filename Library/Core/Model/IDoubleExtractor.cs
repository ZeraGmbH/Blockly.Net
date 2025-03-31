namespace BlocklyNet.Core.Model;

/// <summary>
/// Tag interface for custom classes which finally only
/// carry a double number,
/// /// </summary>
public interface IDoubleExtractor
{
    /// <summary>
    /// Make something a number.
    /// </summary>
    /// <param name="value">Some data.</param>
    /// <returns>The number in it.</returns>
    double GetNumber(object value);
}
