namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 广告表现提升建议
        /// https://developers.facebook.com/docs/marketing-api/overview/performance-recommendations/#applying-recommendations
        /// </summary>
        public class AccountRecommendations
        {
            /// <summary>
            /// 广告表现提升建议列表
            /// </summary>
            public List<AccountRecommendationResult> recommendations { get; set; } = new();
        }

        /// <summary>
        /// 
        /// </summary>
        public class AccountRecommendationResult
        {
            /// <summary>
            /// 这项建议的唯一标识符。是在建议应用 API 中提及该建议的必要项。
            ///  对于在 API 中无法解析的建议，系统不会返回此值。
            /// </summary>
            public string? recommendation_signature { get; set; }

            /// <summary>
            /// 枚举值，
            /// 表示该建议所属的建议类型。
            /// 本文档下方描述了每个可行值的含义以及应用这些值时会发生的情况。
            /// </summary>
            public string? type { get; set; }

            /// <summary>
            /// 本推荐对应的广告对象列表。
            /// 广告对象可以是广告系列、广告组或广告。
            /// </summary>
            public List<string>? object_ids { get; set; }

            /// <summary>
            /// 表示该建议的机会分数提升情况。
            /// </summary>
            public AccountRecommendationContent recommendation_content { get; set; } = new();

            /// <summary>
            ///  账户id
            /// </summary>
            [JsonIgnore]
            public long account_id { get; set; }
        }

        /// <summary>
        /// 表示该建议的机会分数提升情况。
        /// </summary>
        public class AccountRecommendationContent
        {
            /// <summary>
            /// lift_estimate ：描述广告主在接受一个给定建议时可看到的提升情况。
            /// </summary>
            public string? lift_estimate { get; set; }
            /// <summary>
            ///  body :  对该建议的描述，类似于您将在本文档“表现提升建议类型”表中看到的描述。
            /// </summary>
            public string? body { get; set; }

            /// <summary>
            /// opportunity_score_lift ：这是通过应用该建议而有望获得的机会分数提升情况。
            /// </summary>
            public string? opportunity_score_lift { get; set; }
        }

    }

    /*
       AccountRecommendationResult Type 可行值说明 ：
       MUSIC
       
            允许 Meta 自动根据广告内容选择音乐并将其添加到您的广告中，而您无需付费。
            
            应用此推荐，即可为选定的广告对象激活该功能。如果未提供选择，将为所有列出的广告对象启用这项功能。
            
            在您的广告中使用音乐需遵循音频素材条款的规定。
       
       AUTOFLOW_OPT_IN
       
            启用标准美化。在有可能提升广告表现时，标准美化功能会利用 Meta 的数据投放您广告的不同版本。
            
            应用该建议将为所选广告对象启用这项功能。如果未提供选择，将为所有列出的广告对象启用这项功能。
            
            AUTOMATIC_PLACEMENTS
            
            允许 Meta 自动为您的广告组选择更多版位，同时充分利用您的预算。详细了解进阶赋能型版位。
       
       UNCROP_IMAGE
       
            扩展图片以适配更多版位。您可以使用生成的、具有更宽宽高比的图片，这能让您的广告适配新的版位，从而展示给更多受众。
            
            应用该建议将为所选广告对象启用这项功能。如果未提供选择，将为所有列出的广告对象启用这项功能。
       
       FRAGMENTATION
       
            您的一些广告组，其设置和创意相似，但受众群体不同。因此，这些广告组可能需要更长时间才能度过学习阶段，并且在广告效果优化之前会消耗更多预算。如要优化广告花费，请将您的类似广告组合并为一个广告组。
            
            应用该建议将执行以下更改：
            
            表现最佳的那个广告组将保持开启，其余所有广告组都将关闭。该表现最佳的广告组将是 object_ids 中列出的那个广告组。
            已关闭广告组中的定位选项都将合并到表现最佳的广告组中。例如，如果广告组 1 的目标受众是 18-25 岁人群，广告组 2 的目标受众是 35-40 岁人群，则合并广告组的目标受众将是 18-40 岁人群。您的受众更改将包含：“含地区”。
            已关闭广告组中的广告都将并入表现最佳的广告组中。
            已关闭广告组的预算都将添加到表现最佳广告组的预算中。
       ADVANTAGE_SHOPPING_CAMPAIGN_FRAGMENTATION
       
            您的一些广告系列，其设置和创意相似，但受众群体不同。因此，这些广告组可能需要更长时间才能度过学习阶段，并且在广告效果优化之前会消耗更多预算。为避免这种情况，请将您的类似广告系列合并为一个进阶赋能型智能购物广告系列。详细了解进阶赋能型智能购物广告系列。
            
            应用该建议将执行以下更改：
            
            表现最佳的那个进阶赋能型智能购物广告系列将保持开启，而所有其他广告系列都将关闭。如果广告系列中不含任何进阶赋能型智能购物广告系列，系统将新建一个此类广告系列，且所有其他广告系列都将关闭。
            已关闭广告系列的受众都将并入该进阶赋能型智能购物广告系列中。
            已关闭广告系列的预算都将添加到该进阶赋能型智能购物广告系列的预算中。
            已关闭广告系列中的创意都将添加到该进阶赋能型智能购物广告系列中。
       CREATION_PACKAGE_UPGRADE_TO_ASC
       
            您的手动促销广告系列有资格更新为进阶赋能型智能购物广告系列，这可以帮助您最大限度地提升广告表现和发掘新客户。详细了解进阶赋能型智能购物广告系列。
            
            应用该建议将基于现有的促销广告系列新建一个进阶赋能型智能购物广告系列，而现有广告系列将被关闭。
       
       CREATIVE_FATIGUE
       
            此广告组的单次成效费用可能会高于您以往投放的广告组，因为其图片或创意已向部分受众展示了太多次。
            
            应用此建议需要广告编号和创意编号，并将创建所提供广告的副本，但使用提供的新创意。
       
       CREATIVE_LIMITED
       
            此广告组的单次成效费用可能会高于您以往投放的广告组，因为其图片或创意已向部分受众展示了太多次。
            
            应用此建议需要广告编号和创意编号，并将创建所提供广告的副本，但使用提供的新创意。
       
       SIGNALS_GROWTH_CAPI
       
            通过集成转化 API，您可以从 Meta 广告中获得更准确的转化相关数据，从而改进受众定位，帮助降低单次成效费用。
            
            开始使用转化 API。
            
            此建议目前无法通过市场营销 API 应用。
       
       CAPI_PERFORMANCE_MATCH_KEY
       
            在现有的转化报告中发送附加字段可帮助改善广告的表现。
            
            请查看事件管理工具中的 Meta Pixel 像素代码集成。
            
            此建议目前无法通过市场营销 API 应用。
       
       SEMANTIC_BASED_AUDIENCE_EXPANSION
       
            根据同一主题其他广告组的成效，您的广告组有可能获得更多受众的互动，这有助于提高点击率和改善广告表现
       
       SCALE_GOOD_CAMPAIGN
       
            与您或您的同行投放的具有相同优化目标的广告组和广告系列相比，某些广告组或广告系列具有稳定的投放和较低的单次成效费用。增加其预算可进一步扩大成效。
       
       SHOPS_ADS
       
            您账户中的多个广告组使用网站转化发生位置。通过选择网站和店铺的转化发生位置，帮助改善广告表现。这可让您自动将流量发送到您的网站或者 Facebook 或 Instagram 上的店铺。
       
       ADVANTAGE_PLUS_AUDIENCE
       
            利用进阶赋能型受众，让 Meta 自动识别和定位与您广告组最相关的受众细分，从而优化您的预算，实现最大成效。详细了解进阶赋能型受众。
    */
}
