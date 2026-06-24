namespace Ads.Automation.Application.Contracts.IntegrationJobs;

/// <summary>
/// 同步单个广告账户下的报表数据任务参数
/// ReportDate 用于区分日报日期（昨天 / 当日）
/// </summary>
[Ads.Automation.Domain.Shared.Attributes.MessageRoute(Exchange = "ads.automation.jobs", RoutingKey = "SyncAdReportJobArgs")]
public class SyncAdReportJobArgs
{
    /// <summary>
    /// 广告账户编号（Meta account_id）
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 报表日期（yyyy-MM-dd），为空时默认当日
    /// </summary>
    public string? ReportDate { get; set; }
    
    /// <summary>
    /// 日期类型
    /// </summary>
    public string DataType { get; set; } = string.Empty;
    
    public static string Resolve(string? reportDate)
    {
        return reportDate?.ToLowerInvariant() switch
        {
            "yesterday" => DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            "today"     => DateTime.Today.ToString("yyyy-MM-dd"),
            _           => reportDate ?? DateTime.Today.ToString("yyyy-MM-dd")
        };
    }

}
