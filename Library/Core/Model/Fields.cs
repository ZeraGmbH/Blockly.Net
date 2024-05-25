
namespace BlocklyNet.Core.Model;

/// <summary>
/// All fields of a block.
/// </summary>
public class Fields : Entities<Field>
{
    /// <summary>
    /// Get a unique key for the field.
    /// </summary>
    /// <param name="field">Related field.</param>
    /// <returns>Name of the field.</returns>
    protected override string GetKey(Field field) => field.Name;

    /// <summary>
    /// Get the value of a field.
    /// </summary>
    /// <param name="name">Name of the field.</param>
    /// <returns>Value of the field.</returns>
    /// <exception cref="KeyNotFoundException">There is no such key.</exception>
    public string this[string name] => Get(name)!.Value;
}
