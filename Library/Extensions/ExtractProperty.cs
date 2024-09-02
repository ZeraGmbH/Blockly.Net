using System.Collections;
using System.Reflection;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Extract a single property from a JSON object.
/// </summary>
[CustomBlock(
    "extract_property",
    "",
    @"{
        ""message0"": ""ExtractProperty %1 %2 %3 %4 %5"",
        ""args0"": [
        {
            ""type"": ""input_dummy""
        },
        {
            ""type"": ""field_label_serializable"",
            ""name"": ""OBJECT"",
            ""text"": ""Object""
        },
        {
            ""type"": ""input_value"",
            ""name"": ""JSON""
        },
        {
            ""type"": ""field_label_serializable"",
            ""name"": ""PROPERTY"",
            ""text"": ""Property""
        },
        {
            ""type"": ""input_value"",
            ""name"": ""PROPERTY""
        }
        ],
        ""output"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Extract property from JSON Object"",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""PROPERTY"": {
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
public class ExtractProperty : Block
{
    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        /* Get the object and the property. */
        var json = await Values.EvaluateAsync<object>("JSON", context);
        var property = await Values.EvaluateAsync<string>("PROPERTY", context);

        try
        {
            /* Read from a string dictionary - e.g. some ExpandoObject. */
            if (json is IDictionary<string, object> stringDict) return stringDict[property];

            /* Read from generic dictionary. */
            if (json is IDictionary dict)
            {
                /* Check key type. */
                var keyType = dict.GetType().GetGenericArguments()[0];

                return dict[keyType.IsEnum ? Enum.Parse(keyType, property) : property];
            }

            /* Read from .NET instance. */
            return json.GetType().InvokeMember(property, BindingFlags.GetProperty | BindingFlags.GetField, null, json, null)!;
        }
        catch (Exception)
        {
            /* In case of error just report nothing. */
            return null!;
        }
    }
}
