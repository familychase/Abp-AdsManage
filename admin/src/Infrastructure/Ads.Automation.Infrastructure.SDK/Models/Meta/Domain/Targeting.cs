
/*
 * @author: alvin 2022-10-25 19:31
 */

// ReSharper disable InconsistentNaming
using Ads.Automation.Infrastructure.SDK.Models.Meta.Internal;

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// Meta - 目标.
        /// </summary>
        public class Targeting
        {
            /// <summary>
            /// 
            /// </summary>
            public List<string>? device_platforms { get; set; }

            /// <summary>
            /// 发布平台
            /// </summary>
            public List<string>? publisher_platforms { get; set; }

            /// <summary>
            /// facebook定位
            /// </summary>
            public List<string>? facebook_positions { get; set; }

            /// <summary>
            /// instagram定位
            /// </summary>
            public List<string>? instagram_positions { get; set; }

            /// <summary>
            /// audience_network定位
            /// </summary>
            public List<string>? audience_network_positions { get; set; }

            /// <summary>
            /// messenger定位
            /// </summary>
            public List<string>? messenger_positions { get; set; }

            /// <summary>
            /// 最大年龄
            /// </summary>
            public int? age_max { get; set; }

            /// <summary>
            /// 最小年龄
            /// </summary>
            public int? age_min { get; set; }

            /// <summary>
            /// 性别
            /// </summary>
            public List<int>? genders { get; set; }

            /// <summary>
            /// 地理位置设置
            /// </summary>
            public MetaGeoLocationsDto? geo_locations { get; set; }

            /// <summary>
            /// 排除国家
            /// </summary>
            public MetaGeoLocationsDto? excluded_geo_locations { get; set; }

            /// <summary>
            /// 用户系统
            /// </summary>
            public List<string>? user_os { get; set; }

            /// <summary>
            /// 包含的设备
            /// </summary>
            public List<string>? user_device { get; set; }

            /// <summary>
            /// 要排除的设备
            /// </summary>
            public List<string>? excluded_user_device { get; set; }

            /// <summary>
            /// 仅在连接 Wi-Fi 时
            /// </summary>
            public List<string>? wireless_carrier { get; set; }

            /// <summary>
            /// 包含的受众
            /// </summary>
            public List<MetaDictionaryDto>? custom_audiences { get; set; }

            /// <summary>
            /// 排除的受众
            /// </summary>
            public List<MetaDictionaryDto>? excluded_custom_audiences { get; set; }

            /// <summary>
            /// 商品目录的排除信息
            /// </summary>
            public List<AdProductAudienceSpecs>? excluded_product_audience_specs { get; set; }

            /// <summary>
            /// 商品目录的受众信息
            /// </summary>
            public List<AdProductAudienceSpecs>? product_audience_specs { get; set; }

            /// <summary>
            /// 兴趣
            /// </summary>
            public List<MetaDictionaryDto>? interests { get; set; }

            /// <summary>
            /// 行为列表
            /// </summary>
            public List<MetaDictionaryDto>? behaviors { get; set; }

            /// <summary>
            /// 生活记事
            /// </summary>
            public List<MetaDictionaryDto>? life_events { get; set; }

            /// <summary>
            /// 家庭状况
            /// </summary>
            public List<MetaDictionaryDto>? family_statuses { get; set; }

            /// <summary>
            /// 行业信息
            /// </summary>
            public List<MetaDictionaryDto>? industries { get; set; }

            /// <summary>
            /// 收入情况
            /// </summary>
            public List<MetaDictionaryDto>? income { get; set; }

            /// <summary>
            /// 语言
            /// </summary>
            public List<int>? locales { get; set; }

            /// <summary>
            /// 细分定位扩展设置
            /// </summary>
            public string? targeting_optimization { get; set; }

            /// <summary>
            /// 库存筛选方案
            /// </summary>
            public List<string>? brand_safety_content_filter_levels { get; set; }

            /// <summary>
            /// 不在这个广告组中包含可跳过广告
            /// </summary>
            public bool? instream_video_skippable_excluded { get; set; }

            /// <summary>
            /// 内容类型排除条件
            /// </summary>
            public List<string>? excluded_brand_safety_content_types { get; set; }

            /// <summary>
            /// 年龄范围
            /// </summary>
            public List<int>? age_range { get; set; }

            /// <summary>
            /// 进阶赋能型受众，
            /// 启用进阶赋能型受众后，您便可在 targeting_spec 中设置 age_range 参数。
            /// </summary>
            public TargetingAutomation? targeting_automation { get; set; }

            /// <summary>
            ///优势定制受众
            /// </summary>
            public TargetingRelaxationTypes? targeting_relaxation_types { get; set; }

            /// <summary>
            /// 包含的细分定位信息
            /// </summary>
            public List<SubdivisionPositioningSpec>? flexible_spec { get; set; }

            /// <summary>
            /// 排除的细分定位信息
            /// </summary>
            public SubdivisionPositioningSpec? exclusions { get; set; }
        }

        /// <summary>
        /// 优势定制受众
        /// </summary>
        public class TargetingRelaxationTypes
        {
            /// <summary>
            /// 优势相似
            /// https://developers.facebook.com/docs/marketing-api/audiences/reference/targeting-expansion/advantage-lookalike
            /// </summary>
            public int lookalike { get; set; }

            /// <summary>
            /// 优势定制受众
            /// https://developers.facebook.com/docs/marketing-api/audiences/reference/targeting-expansion/advantage-custom-audience
            /// </summary>
            public int custom_audience { get; set; }

        }

        /// <summary>
        /// 进阶赋能受众
        /// https://developers.facebook.com/docs/marketing-api/audiences/reference/targeting-expansion/advantage-audience
        /// </summary>
        public class TargetingAutomation
        {
            /// <summary>
            ///如需启用此选项，请将参数设置为 1。
            ///如需禁用此选项，请将参数设置为 0。
            /// </summary>
            public int advantage_audience { get; set; }
        }

        /// <summary>
        /// 细分定位内容
        /// </summary>
        public class SubdivisionPositioningSpec
        {
            /// <summary>
            /// 兴趣
            /// </summary>
            public List<MetaDictionaryDto>? interests { get; set; }

            /// <summary>
            /// 行为列表
            /// </summary>
            public List<MetaDictionaryDto>? behaviors { get; set; }

            /// <summary>
            /// 生活记事
            /// </summary>
            public List<MetaDictionaryDto>? life_events { get; set; }

            /// <summary>
            /// 家庭状况
            /// </summary>
            public List<MetaDictionaryDto>? family_statuses { get; set; }

            /// <summary>
            /// 行业信息
            /// </summary>
            public List<MetaDictionaryDto>? industries { get; set; }

            /// <summary>
            /// 收入情况
            /// </summary>
            public List<MetaDictionaryDto>? income { get; set; }
        }
    }
}
