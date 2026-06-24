using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ads.Automation.Infrastructure.SDK.Serialization;

/// <summary>
/// Meta API 返回的日期字符串格式自定义反序列化转换器
/// </summary>
public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var dateString = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        // 尝试解析常用日期格式
        if (DateTime.TryParse(dateString, out var result))
            return result;

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ssK"));
    }
}
