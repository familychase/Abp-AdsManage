namespace Ads.Automation.Domain.ApiUsage;

/// <summary>
/// Meta API 调用统计实体（按账户+5分钟槽位）
/// </summary>
public class AdsMetaApiUsage : AggregateRootEntity
{
    public string AccountNo { get; private set; } = string.Empty;
    public DateTime TimeSlot { get; private set; }
    public long TotalCalls { get; private set; }

    /// <summary>
    /// 总消耗积分
    /// </summary>
    public long TotalPoints { get; private set; }

    /// <summary>
    /// 请求方法统计 JSON：{"GET":45,"POST":8,"DELETE":2,"PUT":1}
    /// </summary>
    public string MethodStats { get; private set; } = "{}";

    public long RateLimitHits { get; private set; }
    public DateTime? LastCallTime { get; private set; }
    public DateTime? LastRateLimitTime { get; private set; }
    public DateTime CreationTime { get; private set; }
    public DateTime? LastModificationTime { get; private set; }

    private AdsMetaApiUsage() { }

    public AdsMetaApiUsage(long id, string accountNo, DateTime timeSlot, int points, string method)
        : base(id)
    {
        AccountNo = accountNo;
        TimeSlot = TruncateToSlot(timeSlot);
        TotalCalls = 1;
        TotalPoints = points;
        MethodStats = BuildMethodJson(method);
        LastCallTime = DateTime.Now;
        CreationTime = DateTime.Now;
    }

    public void AddCall(int points, string method)
    {
        TotalCalls++;
        TotalPoints += points;
        LastCallTime = DateTime.Now;
        LastModificationTime = DateTime.Now;

        var stats = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, long>>(MethodStats) ?? new();
        stats.TryGetValue(method, out var count);
        stats[method] = count + 1;
        MethodStats = System.Text.Json.JsonSerializer.Serialize(stats);
    }

    public void IncrementRateLimit()
    {
        RateLimitHits++;
        LastRateLimitTime = DateTime.Now;
        LastModificationTime = DateTime.Now;
    }

    public static DateTime TruncateToSlot(DateTime time)
    {
        var tick = time.Ticks / (TimeSpan.TicksPerMinute * 5) * (TimeSpan.TicksPerMinute * 5);
        return new DateTime(tick, time.Kind);
    }

    private static string BuildMethodJson(string method)
    {
        return $$"""{"{{method}}":1}""";
    }
}
