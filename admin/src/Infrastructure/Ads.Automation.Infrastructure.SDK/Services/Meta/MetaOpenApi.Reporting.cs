
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取广告账户报表数据
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Reporting>> GetReportingAsync(AccessIdentity identity, MetaInput.ReportingQuery query, bool isSync = false)
        {
            var builder = new StringBuilder($"/{query.account_id}/insights?fields={query.fields}");

            if (!string.IsNullOrWhiteSpace(query.breakdowns))
                builder.Append($"&breakdowns={query.breakdowns}");

            if (!string.IsNullOrWhiteSpace(query.time_ranges))
                builder.Append($"&time_ranges={query.time_ranges}");

            if (!string.IsNullOrWhiteSpace(query.action_attribution_windows))
                builder.Append($"&action_attribution_windows={query.action_attribution_windows}");

            if (!string.IsNullOrWhiteSpace(query.level))
                builder.Append($"&level={query.level}");

            if (query.limit != 0)
                builder.Append($"&limit={query.limit}");

            // 异步查询
            if (!isSync)
            {
                // Step 3: 如果异步状态未完成，使用同步请求.
                var request = identity.Request(builder.ToString()).UseVersion(ApiDefaultVersion);
                return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Reporting>>(request))!;
            }
            else
            {
                var request = identity.Request(builder.ToString()).UseVersion(ApiDefaultVersion);

                // 重试5次，等待10分钟，如果仍然失败，查询一次同步
                var executeSecond = 500;
                int totalWaitSeconds = 0;
                var status = string.Empty;
                var taskId = string.Empty;

                for (var i = 1; i <= 2; i++)
                {
                    // Step 1: Post 创建异步请求.
                    var asyncResult = await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.AsyncReportingResult>(request);
                    taskId = asyncResult!.report_run_id;

                    // 等待轮数
                    int waitRow = 1;

                    do
                    {
                        var waitSeconds = WaitSeconds(waitRow++);
                        await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
                        totalWaitSeconds += waitSeconds;

                        // Step 2: 检查异步任务状态.
                        var jobRequest = identity.Request(taskId).UseVersion(ApiDefaultVersion);
                        var checkResult = await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaOutput.AsyncRequestJobState>(jobRequest);

                        status = checkResult?.async_status ?? string.Empty;
                        if (status == "Job Completed")
                        {
                            // Step 3: 查询结果
                            var finalRequest = identity.Request($"{taskId}/insights?limit={query.limit}").UseVersion(ApiDefaultVersion);
                            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Reporting>>(finalRequest))!;
                        }

                        if (status == "Job Failed")
                        {
                            // 异步任务执行失败
                            break;
                        }
                    } while (totalWaitSeconds < executeSecond);

                    if (totalWaitSeconds >= executeSecond)
                    {
                        break;
                    }
                }

                throw new HttpRequestException($"异步请求报表响应失败！{status} {taskId} time: {totalWaitSeconds} {builder}");
            }
        }

        /// <summary>
        /// 获取RestRequest
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static RestRequest GetRequest(AccessIdentity identity, MetaInput.ReportingQuery query)
        {
            var builder = new StringBuilder($"/{query.account_id}/insights?fields={query.fields}");

            if (!string.IsNullOrWhiteSpace(query.breakdowns))
                builder.Append($"&breakdowns={query.breakdowns}");

            if (!string.IsNullOrWhiteSpace(query.time_ranges))
                builder.Append($"&time_ranges={query.time_ranges}");

            if (!string.IsNullOrWhiteSpace(query.action_attribution_windows))
                builder.Append($"&action_attribution_windows={query.action_attribution_windows}");

            if (!string.IsNullOrWhiteSpace(query.level))
                builder.Append($"&level={query.level}");

            if (query.limit != 0)
                builder.Append($"&limit={query.limit}");

            var request = identity.Request(builder.ToString()).UseVersion(ApiDefaultVersion);

            return request;
        }


        /// <summary>
        /// 创建异步请求.
        /// 得到TaskId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<MetaOutput.AsyncReportingResult> CreateReportingTaskAsync(RestRequest request)
        {
            var asyncResult = await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.AsyncReportingResult>(request);
            return asyncResult!;
        }

        /// <summary>
        /// 检查异步任务状态.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static async Task<MetaOutput.AsyncRequestJobState> CheckReportingTaskAsync(AccessIdentity identity, string taskId)
        {
            var jobRequest = identity.Request(taskId).UseVersion(ApiDefaultVersion);
            var checkResult = await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaOutput.AsyncRequestJobState>(jobRequest);
            return checkResult!;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="taskId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Reporting>> GetReportingDataAsync(AccessIdentity identity, string taskId, int limit)
        {
            var finalRequest = identity.Request($"{taskId}/insights?limit={limit}").UseVersion(ApiDefaultVersion);
            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Reporting>>(finalRequest))!;
        }

        /// <summary>
        /// 获取等待时长秒
        /// </summary>
        /// <param name="row">循环到第几轮</param>
        /// <returns></returns>
        private static int WaitSeconds(int row)
        {
            return row switch
            {
                1 => 5,
                2 => 10,
                3 => 15,
                4 => 20,
                _ => 30
            };
        }
    }
}
