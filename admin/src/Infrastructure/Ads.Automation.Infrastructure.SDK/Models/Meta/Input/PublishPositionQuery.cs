/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{

    public partial class MetaInput
    {
        public class PublishPositionQuery
        {
            /// <summary>
            /// 账户信息
            /// </summary>
            public string? account_id { get; set; }

            /// <summary>
            /// 计费事件
            /// </summary>
            public string? billing_event { get; set; }

            /// <summary>
            /// 购买类型
            /// </summary>
            public string? buying_type { get; set; }

            /// <summary>
            /// 目标
            /// </summary>
            public string? objective { get; set; }

            /// <summary>
            /// 优化目标
            /// </summary>
            public string? optimization_goal { get; set; }
        }
    }
}
