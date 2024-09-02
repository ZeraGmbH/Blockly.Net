using System.Collections;
using System.Reflection;
using BlocklyNet.Core.Blocks.Lists;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Read a single property from a model.
/// </summary>
[CustomBlock(
    "read_model_property",
    "",
    @"{
        ""message0"": ""ReadFromModel %1 %2 %3 %4 %5 %6"",
        ""args0"": [
            {
                ""type"": ""field_variable"",
                ""name"": ""VAR"",
                ""text"": ""Variable""
            },
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""PATH"",
                ""text"": ""Path to value""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PATH"",
                ""check"": ""String""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""INDEXES"",
                ""text"": ""Indexes in path""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""INDEXES"",
                ""check"": [""Array(Number)"", ""Array""]
            }
        ],
        ""output"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Read a single field from a model variable"",
        ""helpUrl"": """"
    }",
    @"{
      ""inputs"": {
        ""PATH"": {
            ""shadow"": {
              ""type"": ""text"",
              ""fields"": {
                ""TEXT"": """"
              }
            }
        }
      }
    }"
)]
public class ReadFromModel : Block
{
    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        var data = context.Variables[Fields["VAR"]];
        var path = await Values.EvaluateAsync<string>("PATH", context) ?? "";
        var rawIndexes = await Values.EvaluateAsync<IEnumerable>("INDEXES", context, false);
        var indexes = rawIndexes?.Cast<object>().ToArray() ?? [];

        var parts = path.Split(".");
        var i = 0;

        foreach (var part in parts)
        {
            var isArray = part.EndsWith("[]");
            var field = isArray ? part[..^2] : part;

            /* Read from a string dictionary - e.g. some ExpandoObject. */
            if (data is IDictionary<string, object> stringSource)
                data = stringSource[field];

            /* Read from generic dictionary. */
            else if (data is IDictionary source)
            {
                /* Check key type. */
                var keyType = source.GetType().GetGenericArguments()[0];

                data = source[keyType.IsEnum ? Enum.Parse(keyType, field) : field];
            }

            /* Read from .NET instance. */
            else
                data = data!.GetType().InvokeMember(field, BindingFlags.GetProperty | BindingFlags.GetField, null, data, null)!;

            /* Resolve index. */
            if (isArray)
                data = new ListWrapper(data)[Convert.ToInt32(indexes[i++]) - 1];
        }

        return data;
    }
}
