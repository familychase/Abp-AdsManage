/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 落地页详细信息
        /// </summary>
        public class Pixel
        {
            /// <summary>
            /// 应用id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 应用名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 落地页的Js代码
            /// </summary>
            public string? code { get; set; }

            /// <summary>
            /// 像素事件统计
            /// </summary>
            public MetaPagedDto<PixelEventStatsList>? stats { get; set; } = null;
        }

        /// <summary>
        /// 像素事件统计
        /// </summary>
        public class PixelEventStatsList
        {
            /// <summary>
            /// 
            /// </summary>
            public DateTime start_time { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string? aggregation { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public IList<PixelEventStats>? data { get; set; } = null;
        }

        public class PixelEventStats
        {
            /// <summary>
            /// 事件名称
            /// </summary>
            public string? value { get; set; } = null;

            /// <summary>
            /// 统计次数
            /// </summary>
            public int count { get; set; }
        }

        /// <summary>
        /// 落地页详细信息
        /// </summary>
        public class PixelCustomConversions
        {
            /// <summary>
            /// 转化id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 转化名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 自定义事件类型
            /// </summary>
            public string? custom_event_type { get; set; }

            /// <summary>
            /// 自定义转换规则
            /// </summary>
            public string? rule { get; set; }

            /// <summary>
            /// 将发送事件的像素
            /// </summary>
            public Pixel? pixel { get; set; }
        }
    }
}
