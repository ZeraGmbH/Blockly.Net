using System.Collections;
using System.Reflection;
using BlocklyNet.Core.Blocks.Lists;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Update a single property of a model.
/// </summary>
[CustomBlock(
    "update_model_property",
    "",
    @"{
        ""message0"": ""UpdateModelProperty %1 %2 %3 %4 %5 %6 %7 %8"",
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
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""VALUE"",
                ""text"": ""Value""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""VALUE""
            }
        ],
        ""previousStatement"": null,
        ""nextStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Update a single field of a model variable"",
        ""helpUrl"": """"
    }",
    @"{
      ""inputs"": {
        ""PATH"": {
            ""shadow"": {
              ""type"": ""text"",
              ""fields"": {
                ""VALUE"": """"
              }
            }
        }
      }
    }"
)]
public class UpdateModelProperty : Block
{
    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        var data = context.Variables[Fields["VAR"]];
        var value = await Values.Evaluate("VALUE", context);
        var path = await Values.Evaluate<string>("PATH", context) ?? "";
        var rawIndexes = await Values.Evaluate<IEnumerable>("INDEXES", context, false);
        var indexes = rawIndexes?.Cast<object>().ToArray() ?? [];

        var parts = path.Split(".");
        var i = 0;

        foreach (var part in parts.Take(parts.Length - 1))
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

        var leaf = parts[^1];

        /* Write to a string dictionary - e.g. some ExpandoObject. */
        if (data is IDictionary<string, object> stringTarget)
            stringTarget[leaf] = value!;

        /* Write to generic dictionary. */
        else if (data is IDictionary target)
        {
            /* Check key type. */
            var keyType = target.GetType().GetGenericArguments()[0];

            target[keyType.IsEnum ? Enum.Parse(keyType, leaf) : leaf] = value;
        }

        /* Write to .NET instance. */
        else
            data!.GetType().InvokeMember(leaf, BindingFlags.SetProperty | BindingFlags.SetField, null, data, [value]);

        return await base.Evaluate(context);
    }
}
