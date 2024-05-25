namespace BlocklyNet.Core.Model;

/// <summary>
/// A static field.
/// </summary>
public class Field
{
    /// <summary>
    /// Unique name of the field.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Value of the field - always a string which must 
    /// be converted accordingly.
    /// </summary>
    public required string Value { get; set; }
}

