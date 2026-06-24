
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class AdPromotedObject
        {
            /// <summary>
            /// Facebook应用程序的ID。通常与在Facebook上推广的手机或帆布游戏有关
            /// </summary>
            public string application_id { get; set; } = null!;

            /// <summary>
            /// 可以购买/下载应用程序的移动/数字商店的uri。这是平台特有的。当与"application_id"结合时，这个对象可以是一个Facebook广告活动的主题
            /// </summary>
            public string? object_store_url { get; set; }

            /// <summary>
            /// 标准事件
            /// </summary>
            public string custom_event_type { get; set; } = null!;

            /// <summary>
            /// 自定义事件，custom_event_type设置为OTHER
            /// </summary>
            public string custom_event_str { get; set; } = null!;

            /// <summary>
            /// Facebook像素Id
            /// </summary>
            public string? pixel_id { get; set; }

            /// <summary>
            /// 商品目录Id，在目录广告的广告系列中使用
            /// </summary>
            public string? product_catalog_id { get; set; }

            /// <summary>
            /// 商品系列Id，在目录广告的广告组中使用
            /// </summary>
            public string?  product_set_id { get; set; }

            /// <summary>
            /// 像素自定义事件匹配规则
            /// </summary>
            public string? pixel_rule { get; set; }

            /// <summary>
            /// 自定义转化id
            /// </summary>
            public string? custom_conversion_id { get; set; }
        }
    }
}
