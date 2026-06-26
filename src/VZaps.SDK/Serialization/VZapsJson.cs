using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VZaps.Serialization;

internal static class VZapsJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = VZapsSnakeCaseNamingPolicy.Instance,
        DictionaryKeyPolicy = VZapsSnakeCaseNamingPolicy.Instance,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(VZapsSnakeCaseNamingPolicy.Instance) },
    };

    public static readonly JsonSerializerOptions RealtimeOptions = new()
    {
        PropertyNamingPolicy = VZapsSnakeCaseNamingPolicy.Instance,
        DictionaryKeyPolicy = VZapsSnakeCaseNamingPolicy.Instance,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}

internal sealed class VZapsSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static readonly VZapsSnakeCaseNamingPolicy Instance = new();

    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var builder = new StringBuilder(name.Length + 8);
        for (var i = 0; i < name.Length; i++)
        {
            var character = name[i];
            if (char.IsUpper(character))
            {
                if (i > 0 && name[i - 1] != '_' && (!char.IsUpper(name[i - 1]) || (i + 1 < name.Length && char.IsLower(name[i + 1]))))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
            }
            else
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
