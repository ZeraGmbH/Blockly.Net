using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Core;

/// <summary>
/// 
/// </summary>
public interface IParserConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TParser"></typeparam>
    void Configure<TParser>(BlocklyExtensions.BlockBuilder<TParser> builder) where TParser : Parser<TParser>;
}
