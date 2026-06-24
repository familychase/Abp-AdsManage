namespace Ads.Automation.Domain.Publishing;

/// <summary>
/// Publishing 模块通用扩展方法
/// </summary>
public static class PublishingExtensions
{
    /// <summary>
    /// 判断集合是否不为 null 且包含元素
    /// </summary>
    public static bool NotNullOrEmpty<T>(this IEnumerable<T>? source) => source != null && source.Any();

    /// <summary>
    /// 判断对象是否不为 null
    /// </summary>
    public static bool IsNotNull(this object? obj) => obj != null;

    /// <summary>
    /// 判断字符串是否不为 null 或空
    /// </summary>
    public static bool NotNullOrEmpty(this string? value) => !string.IsNullOrEmpty(value);

    /// <summary>
    /// 判断对象是否为 null
    /// </summary>
    public static bool IsNull(this object? obj) => obj == null;

    /// <summary>
    /// 判断字符串是否为 null 或空
    /// </summary>
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
}
