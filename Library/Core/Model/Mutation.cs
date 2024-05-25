

namespace BlocklyNet.Core.Model;

public class Mutation(string domain, string name, string value)
{
    public string Domain { get; set; } = domain;
    public string Name { get; set; } = name;
    public string Value { get; set; } = value;

}

