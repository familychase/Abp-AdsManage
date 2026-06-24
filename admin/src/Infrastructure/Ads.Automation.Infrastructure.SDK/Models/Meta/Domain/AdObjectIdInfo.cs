
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {

        /// <summary>
        /// 复制模型
        /// </summary>
        public class AdObjectIdInfo
        {
            /// <summary>
            /// 广告对象类型
            /// 枚举：unique_adcreative, ad, ad_set, campaign, opportunities, privacy_info_center, topline, ad_account
            /// </summary>
            public string ad_object_type { get; set; } = null!;

            /// <summary>
            /// 复制来源Id
            /// </summary>
            public string source_id { get; set; }=null!;

            /// <summary>
            /// 复制后的Id
            /// </summary>
            public string copied_id { get; set; }=null !;
        }
    }

}
