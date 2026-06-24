using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ads.Automation.Domain.Shared;

public static class JsonHelper
{
    public static string ToJsonIgnoreNullValue<T>(this T data)
    {
        return JsonSerializer.Serialize(data, IgnoreNullJsonOptions);
    }
    
    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = null!,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Converters = { new CustomDateTimeConverter() }
    };
    
    public static readonly JsonSerializerOptions IgnoreNullJsonOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Converters = { new CustomDateTimeConverter() } };
    
    /// <summary>
    /// json时间格式  格式化
    /// </summary>
    public class CustomDateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out DateTime dateTime))
                return dateTime;
            else
                throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}