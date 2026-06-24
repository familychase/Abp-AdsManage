/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class PromotePage
        {
            /// <summary>
            /// id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 名称
            /// </summary>
            public string? name { get; set; } = null!;

            /// <summary>
            /// 主页分类名称，字段在获取用户级主页时应用
            /// </summary>
            public string? category { get; set; }
        }
    }
}
