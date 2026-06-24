
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{
    public partial class MetaInput
    {
        public class ReportingQuery
        {
            /// <summary>
            /// 广告账户Id
            /// </summary>
            public string account_id { get; set; } = null!;

            public string? breakdowns { get; private set; }

            public string fields { get; set; } = null!;

            public string? action_attribution_windows { get; private set; }

            public string time_ranges { get; private set; } = null!;

            public int limit { get; set; }

            public string? level { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="p"></param>
            public void SetBreakdowns(IEnumerable<string> p)
            {
                breakdowns = JsonSerializer.Serialize(p);
            }

            /// <summary>
            /// 设置归因窗口
            /// </summary>
            /// <param name="p"></param>
            public void SetAttributionWindows(IEnumerable<string> p = null!)
            {
                //默认值
                p ??= new List<string>() { "1d_click", "7d_click", "28d_click", "1d_view", "7d_view", "28d_view", "1d_ev" };
                action_attribution_windows = JsonSerializer.Serialize(p);
            }

            /// <summary>
            /// 设置查询时间范围.
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            public void SetTimeRanges(DateTime start, DateTime end)
            {
                var builder = new StringBuilder();

                while (start <= end)
                {

                    builder.Append($"{{'since':'{start:yyyy-MM-dd}','until':'{start:yyyy-MM-dd}'}},");
                    start = start.AddDays(1);
                }

                builder.Remove(builder.Length - 1, 1);

                time_ranges = $"[{builder}]";
            }


            /// <summary>
            /// 设置查询总时间范围.
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            public void SetTotalTimeRanges(DateTime start, DateTime end)
            {
                time_ranges = $"[{{'since':'{start:yyyy-MM-dd}','until':'{end:yyyy-MM-dd}'}}]";
            }


        }
    }
}
