
using System.Text.Json.Serialization;
using Ads.Automation.Infrastructure.SDK.Serialization;

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 区域监管身份
        /// </summary>
        public class AdSetRegionalRegulationIdentities
        {
            /// <summary>
            /// 台湾金融受益人
            /// </summary>
            public string? taiwan_finserv_beneficiary { get; set; } = null!;

            /// <summary>
            /// 台湾金融付费人
            /// </summary>
            public string? taiwan_finserv_payer { get; set; } = null!;

            /// <summary>
            /// 澳大利亚金融受益人
            /// </summary>
            public string? australia_finserv_beneficiary { get; set; } = null!;

            /// <summary>
            /// 澳大利亚金融付费人
            /// </summary>
            public string? australia_finserv_payer { get; set; } = null!;

            /// <summary>
            /// 台湾受益人
            /// </summary>
            public string? taiwan_universal_beneficiary { get; set; } = null!;

            /// <summary>
            /// 台湾付费人
            /// </summary>
            public string? taiwan_universal_payer { get; set; } = null!;

            /// <summary>
            /// 新加坡受益人
            /// </summary>
            public string? singapore_universal_beneficiary { get; set; } = null!;

            /// <summary>
            /// 新加坡付费人
            /// </summary>
            public string? singapore_universal_payer { get; set; } = null!;
        }

        /// <summary>
        /// Meta - AdSet.
        /// </summary>
        public class AdSet
        {
            /// <summary>
            /// 广告组ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 广告账户Id
            /// </summary>
            public string account_id { get; set; } = null!;

            /// <summary>
            /// 广告系列Id
            /// </summary>
            public string campaign_id { get; set; } = null!;

            /// <summary>
            /// 广告组名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 广告组开始时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? start_time { get; set; }

            /// <summary>
            /// 广告组结束时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? end_time { get; set; }

            /// <summary>
            /// 广告设置时间表。
            /// </summary>
            public List<AdsetSchedule>? adset_schedule { get; set; }

            /// <summary>
            /// 广告组状态
            /// enum {ACTIVE, PAUSED, DELETED, ARCHIVED}
            /// </summary>
            public string? status { get; set; }

            /// <summary>
            /// 广告组更新时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? updated_time { get; set; }

            /// <summary>
            /// 广告组创建时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? created_time { get; set; }


            /// <summary>
            /// 出价
            /// </summary>
            public int? bid_amount { get; set; }

            /// <summary>
            /// 出价策略
            /// 枚举：LOWEST_COST_WITHOUT_CAP，LOWEST_COST_WITH_BID_CAP，COST_CAP
            /// </summary>
            public string? bid_strategy { get; set; }

            /// <summary>
            /// 广告组每日预算
            /// </summary>
            public string? daily_budget { get; set; }
            /// <summary>
            /// 广告组在总预算
            /// </summary>
            public string? lifetime_budget { get; set; }

            /// <summary>
            /// 每天最小消耗目标
            /// </summary>
            public long? daily_min_spend_target { get; set; }

            /// <summary>
            /// 每日消费上限
            /// </summary>
            public long? daily_spend_cap { get; set; }

            /// <summary>
            /// 终身最小消耗目标
            /// </summary>
            public long? lifetime_min_spend_target { get; set; }

            /// <summary>
            /// 终身消耗上限
            /// </summary>
            public long? lifetime_spend_cap { get; set; }

            /// <summary>
            /// 投放类型，默认为标准或使用广告调度
            /// </summary>
            public List<string> pacing_type { get; set; } = null!;

            /// <summary>
            /// 优化目标
            /// </summary>
            public string? optimization_goal { get; set; }

            /// <summary>
            /// 计费事件
            /// </summary>
            public string? billing_event { get; set; }

            /// <summary>
            /// 是否为动态素材
            /// </summary>
            public bool? is_dynamic_creative { get; set; }

            /// <summary>
            /// 广告归因方法
            /// </summary>
            public string? campaign_attribution { get; set; }

            /// <summary>
            /// 定位
            /// </summary>
            public string? destination_type { get; set; }

            /// <summary>
            /// 广告学习阶段
            /// </summary>
            public LearningStageInformation? learning_stage_info { get; set; }

            /// <summary>
            /// 归因
            /// </summary>
            public List<AttributionSpec>? attribution_spec { get; set; }

            /// <summary>
            /// 目标
            /// </summary>
            public Targeting? targeting { get; set; }

            /// <summary>
            /// 推广的对象
            /// </summary>
            public AdPromotedObject? promoted_object { get; set; }

            /// <summary>
            /// 优化目标为价值时传入
            /// </summary>
            public BidContraint? bid_constraints { get; set; }

            /// <summary>
            /// 该广告集中所有广告的受益人
            /// </summary>
            public string? dsa_beneficiary { get; set; } 

            /// <summary>
            /// 该广告集中所有广告的付款人
            /// </summary>
            public string? dsa_payor { get; set; }

            /// <summary>
            /// 广告系列
            /// </summary>
            public AdCampaign? campaign { get; set; }

            /// <summary>
            /// 异常信息
            /// </summary>
            public List<IssuesInfo>? issues_info { get; set; }

            /// <summary>
            /// 区域监管身份
            /// 此参数用于指定用于表示广告组的region_regulation_identities。目前它支持6个字段：
            /// taiwan_finserv_beneficiary：用于 TAIWAN_FINSERV 类别
            /// taiwan_finserv_payer：用于 TAIWAN_FINSERV 类别
            /// australia_finserv_beneficiary：用于 AUSTRALIA_FINSERV 类别
            /// australia_finserv_payer：用于 AUSTRALIA_FINSERV 类别
            /// taiwan_universal_beneficiary：用于 TAIWAN_UNIVERSAL 类别
            /// taiwan_universal_payer：用于 TAIWAN_UNIVERSAL 类别
            /// </summary>
            public AdSetRegionalRegulationIdentities? regional_regulation_identities { get; set; }

            /// <summary>
            /// 此参数用于指定regional_regulated_categories。目前支持null三个值：
            ///  TAIWAN_FINSERV：如果广告定位到台湾受众，请使用此值声明金融服务广告组
            ///  AUSTRALIA_FINSERV：如果广告组定位到澳大利亚受众，请使用此值声明金融服务广告组
            ///  TAIWAN_UNIVERSAL：如果广告组针对的是台湾受众，请使用此值来声明广告组
            ///  SINGAPORE_UNIVERSAL ：如果广告组针对新加坡受众，请使用此值来声明该广告组
            ///  如果广告组是金融服务广告，并且定位到台湾，则需要TAIWAN_FINSERV声明TAIWAN_UNIVERSAL
            ///  例如：null或[AUSTRALIA_FINSERV] 或[TAIWAN_FINSERV, TAIWAN_UNIVERSAL]
            /// </summary>
            public List<string>? regional_regulated_categories { get; set; }

            /// <summary>
            /// 推广活动是否应使用增量归因优化。
            /// </summary>
            public bool? is_incremental_attribution_enabled { get; set; }

            /// <summary>
            /// 复制设置参数为空参数
            /// </summary>
            /// <param name="smart_promotion_type"></param>
            public void SetParameterIsNull(string smart_promotion_type)
            {
                this.id = null!;

                //去除广告组受众信息
                this.targeting!.custom_audiences = null;
                this.targeting!.excluded_custom_audiences = null;

                this.is_dynamic_creative = this.is_dynamic_creative.HasValue && this.is_dynamic_creative!.Value ? true : null!;

                //应用进阶赋能
                if (smart_promotion_type == "SMART_APP_PROMOTION")//$"{AdsPublishingSmartPromotionType.SMART_APP_PROMOTION}")
                {
                    this.attribution_spec = null!;

                    SetSmartPromotionTypeIsNull();

                    //应用进阶赋能未设置优化目标为价值  不能设置投放速度
                    if (this.optimization_goal != "VALUE")
                    {
                        this.pacing_type = null!;
                    }

                    this.targeting.brand_safety_content_filter_levels = null!;
                }

                //销量进阶赋能 不能设置投放速度
                if (smart_promotion_type == $"AUTOMATED_SHOPPING_ADS")
                {
                    SetSmartPromotionTypeIsNull();

                    this.pacing_type = null!;
                }

                //广告组开启时段不能设置投放速度
                if (this.adset_schedule != null && this.adset_schedule.Any())
                {
                    this.pacing_type = null!;
                }
            }

            /// <summary>
            /// 进阶赋能空处理设置
            /// </summary>
            public void SetSmartPromotionTypeIsNull()
            {
                //无法设置最大年龄
                this.targeting!.age_min = null!;
                this.targeting.age_max = null!;

                this.targeting.user_os = null!;
                this.targeting.user_device = null!;
                this.targeting.excluded_user_device = null!;
                this.targeting.wireless_carrier = null!;

                this.daily_min_spend_target = null!;
                this.daily_spend_cap = null!;
                this.lifetime_min_spend_target = null!;
                this.lifetime_spend_cap = null!;

                this.targeting.geo_locations!.location_types = null!;

                this.targeting.targeting_relaxation_types = null!;
            }
        }

        public class LearningStageInformation
        {
            /// <summary>
            /// 学习阶段
            /// </summary>
            public string? status { get; set;  }

            /// <summary>
            /// 上次
            /// </summary>
            public long? last_sig_edit_ts { get; set; }

            /// <summary>
            /// 距离上一次窗口之后的转化次数
            /// </summary>
            public long? conversions { get; set; }

            /// <summary>
            /// 归因窗口设置
            /// </summary>
            public List<string>? attribution_windows { get; set; }

        }

    }

}
