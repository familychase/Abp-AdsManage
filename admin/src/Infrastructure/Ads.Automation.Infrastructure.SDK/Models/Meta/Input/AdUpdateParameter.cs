/*
 * @author: huangk 2022-12-7 18:41:43
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{

    public partial class MetaInput
    {

        /// <summary>
        /// 更新广告参数
        /// </summary>
        public class AdUpdateParameter
        {

            /// <summary>
            /// 标签
            /// </summary>
            public List<object> ?adlabels { get; set; } 

            /// <summary>
            /// 受众ID
            /// </summary>
            public string? audience_id { get; set; } 

            /// <summary>
            /// 此广告的出价金额
            /// </summary>
            public long? bid_amount { get; set; }

            /// <summary>
            /// 发生转换的域
            /// </summary>
            public string? conversion_domain { get; set; }

            /// <summary>
            /// 广告创意，必须的
            /// </summary>
            public MetaCreativeDto? creative { get; set; }

            /// <summary>
            /// 同一广告活动中广告的顺序
            /// </summary>
            public long? display_sequence { get; set; }

            /// <summary>
            /// 广告组草稿的ID
            /// </summary>
            public long? draft_adgroup_id { get; set; }

            /// <summary>
            /// 是否创建新的受众
            /// </summary>
            public Boolean? engagement_audience { get; set; }

            /// <summary>
            /// 当指定了这个选项时，API调用不会执行变异，而是会对每个字段的值执行验证规则。
            /// </summary>
            public List<string>? execution_options { get; set; } 

            /// <summary>
            /// 广告名称
            /// </summary>
            public string ?name { get; set; }

            /// <summary>
            /// 状态
            /// enum{ACTIVE, PAUSED, DELETED, ARCHIVED}
            /// </summary>
            public string? status { get; set; } 

            /// <summary>
            /// 使用Tracking Specs，你可以记录人们在你的广告上所采取的行动
            /// </summary>
            public object? tracking_specs { get; set; }


            /// <summary>
            /// 修改广告状态
            /// </summary>
            public void SetStatus(string adStatus)
            {
                this.status = adStatus;
            }
        }
    }
}
