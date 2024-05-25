using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Core;

public interface IParserConfiguration
{
    void Configure<TParser>(BlocklyExtensions.BlockBuilder<TParser> builder) where TParser : Parser<TParser>;
}
