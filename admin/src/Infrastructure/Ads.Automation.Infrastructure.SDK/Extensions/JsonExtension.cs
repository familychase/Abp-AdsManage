using System.Text.Json;

namespace Ads.Automation.Infrastructure.SDK.Extensions
{
    /// <summary>
    /// JSON序列化扩展方法
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// 将对象序列化为格式化的JSON字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>格式化的JSON字符串</returns>
        public static string ToFormattedJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// 将对象序列化为JSON字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="writeIndented">是否格式化输出</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson<T>(this T obj, bool writeIndented = false)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = writeIndented
            });
        }
    }
}