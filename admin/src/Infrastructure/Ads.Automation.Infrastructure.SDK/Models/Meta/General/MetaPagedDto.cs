// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.General
{
    public class MetaPagedDto<T>
    {
        /// <summary>
        /// 结果集
        /// </summary>
        public IList<T>? data { get; set; } = null!;

        /// <summary>
        /// 分页参数传输模型.
        /// </summary>
        public MetaPagingDto? paging { get; set; } = null!;

        /// <summary>
        /// 合计个数
        /// </summary>
        public MetaSummaryDto? summary { get; set; } = null!;
    }


    public class MetaPagingDto
    {
        /// <summary>
        /// 
        /// </summary>
        public MetaPagingCursorDto cursors { get; set; } = null!;

        /// <summary>
        /// 下一页
        /// </summary>
        public string? next { get; set; }

        public class MetaPagingCursorDto
        {
            /// <summary>
            /// 
            /// </summary>
            public string before { get; set; } = null!;

            /// <summary>
            /// 
            /// </summary>
            public string after { get; set; } = null!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MetaPagingSummaryDto<T>: MetaPagedDto<T>
    {
        /// <summary>
        /// 合计个数
        /// </summary>
        public MetaSummaryDto? summary { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MetaSummaryDto
    {
        /// <summary>
        /// 总个数
        /// </summary>
        public int total_count { get; set; }
    }
}
