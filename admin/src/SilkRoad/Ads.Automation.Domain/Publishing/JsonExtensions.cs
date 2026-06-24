using System.Text.Json;

namespace Ads.Automation.Domain.Publishing;

/// <summary>
/// JSON 序列化扩展
/// </summary>
public static class JsonExtensions
{
    public static string ToJsonIgnoreNullValue(this object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }
}
