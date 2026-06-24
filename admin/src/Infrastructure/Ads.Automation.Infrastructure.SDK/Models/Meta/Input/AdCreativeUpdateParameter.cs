/*
 * @author: huangk 2022-12-7 18:41:43
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{
    public partial class MetaInput
    {
        /// <summary>
        /// 更新广告创意
        /// </summary>
        public class AdCreativeUpdateParameter
        {

            /// <summary>
            /// 账户ID
            /// </summary>
            public string account_id { get; set; } = null!;

            /// <summary>
            /// 标签
            /// </summary>
            public List<object>? adlabels { get; set; } 


            /// <summary>
            /// 名称
            /// </summary>
            public string? name { get; set; }


            /// <summary>
            /// 状态
            /// enum {ACTIVE, IN_PROCESS, WITH_ISSUES, DELETED}
            /// </summary>
            public string? status { get; set; } 

        }
    }
}
