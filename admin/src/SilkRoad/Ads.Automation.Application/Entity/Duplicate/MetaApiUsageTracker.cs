
namespace Ads.Automation.Application.Entity.Duplicate;

/// <summary>
/// Meta API 积分统计 — 写入 ads_meta_api_usage 表
/// MethodStats 列存 JSON: {"GET":45,"POST":8}
/// 同时监听两条回调路径：
///   1. RestRequestExtension.OnHttpRequest（ConcurrencyExecuteAsync 直接调用 → 已知具体 HTTP method）
///   2. MetaApiRetryPolicy.OnApiExecuted（_retry.ExecuteAsync 带重试调用 → 从 operation 名称推断）
/// </summary>
public class MetaApiUsageTracker : ISingletonDependency
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MetaApiUsageTracker(MetaApiRetryPolicy retryPolicy, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        // 路径 1：ConcurrencyExecuteAsync 直接调用 → 已知具体 HTTP method
        RestRequestExtension.OnHttpRequest = Track;

        // 路径 2：MetaApiRetryPolicy.ExecuteAsync 带重试调用 → 从 operation 名称推断
        retryPolicy.OnApiExecuted = (accountNo, operation, isRateLimit) =>
        {
            var (method, points) = InferMethodAndPoints(operation);
            Track(accountNo, method, points, isRateLimit);
        };
    }

    /// <summary>
    /// 根据操作名推断 HTTP 方法和积分消耗
    /// 写操作 → POST 3分 | 删除 → DELETE 3分 | 更新 → PUT 3分 | 读取 → GET 1分
    /// </summary>
    private static (string method, int points) InferMethodAndPoints(string operation)
    {
        if (operation.Contains("创建") || operation.Contains("上传") || operation.Contains("复制") || operation.Contains("提交"))
            return ("POST", 3);

        if (operation.Contains("删除"))
            return ("DELETE", 3);

        if (operation.Contains("更新") || operation.Contains("修改"))
            return ("PUT", 3);

        return ("GET", 1);
    }

    private void Track(string accountNo, string method, int points, bool isRateLimit)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IAdsMetaApiUsageRepository>();
                var slot = AdsMetaApiUsage.TruncateToSlot(DateTime.Now);
                var existing = await repo.FindBySlotAsync(accountNo, slot);

                if (existing != null)
                {
                    existing.AddCall(points, method);
                    if (isRateLimit) existing.IncrementRateLimit();
                    await repo.UpdateAsync(existing);
                }
                else
                {
                    var record = new AdsMetaApiUsage(IdGenerator.GetNextId(), accountNo, DateTime.Now, points, method);
                    if (isRateLimit) record.IncrementRateLimit();
                    await repo.InsertAsync(record);
                }
            }
            catch { /* 统计失败不影响主流程 */ }
        });
    }
}
