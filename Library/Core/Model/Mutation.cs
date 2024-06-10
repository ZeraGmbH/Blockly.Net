

namespace BlocklyNet.Core.Model;

/// <summary>
/// 
/// </summary>
public class Mutation(string domain, string name, string value)
{
    /// <summary>
    /// 
    /// </summary>
    public string Domain { get; set; } = domain;

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 
    /// </summary>
    public string Value { get; set; } = value;
}

