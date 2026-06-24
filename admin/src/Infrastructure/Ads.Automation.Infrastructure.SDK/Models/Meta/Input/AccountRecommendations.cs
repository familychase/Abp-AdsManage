
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{
    public partial class MetaInput
    {
        /// <summary>
        /// 广告表现提升建议
        /// https://developers.facebook.com/docs/marketing-api/overview/performance-recommendations/#applying-recommendations
        /// </summary>
        public class AccountRecommendations
        {
            /// <summary>
            /// 必需。
            /// 建议获取 API 中提供的签名，该签名对应于一个独特的建议。
            /// </summary>
            public string recommendation_signature { get; set; }

            /// <summary>
            /// 音乐建议参数。下方列出了具体参数。
            /// </summary>
            public AccountRecommendationsAdObjectParameters? music_parameters { get; set; }

            /// <summary>
            /// 自动流程选择启用建议参数。下方列出了具体参数。
            /// </summary>
            public AccountRecommendationsAdObjectParameters? autoflow_parameters { get; set; }

            /// <summary>
            /// 受众碎片化建议参数。下方列出了具体参数。
            /// </summary>
            public AccountRecommendationsAdObjectParameters? fragmentation_parameters { get; set; }

            /// <summary>
            /// 进阶赋能型智能购物广告系列受众碎片化建议参数。下方列出了具体参数。
            /// </summary>
            public AccountRecommendationsAdObjectParameters? asc_fragmentation_parameters { get; set; }


            public class AccountRecommendationsAdObjectParameters
            {
                public AccountRecommendationsAdObjectParameters()
                {
                }

                public AccountRecommendationsAdObjectParameters(List<string> objectSelection)
                {
                    object_selection = objectSelection;
                }
                
                public List<string>? object_selection { get; set; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="suggestionType"></param>
            /// <param name="nos"></param>
            public void SetParameter(string suggestionType,List<string> nos)
            {
                var model = new AccountRecommendationsAdObjectParameters(nos);
                
                if (suggestionType == "MUSIC")
                {
                    this.music_parameters = model;
                }

                if (suggestionType == "AUTOFLOW_OPT_IN")
                {
                    this.autoflow_parameters = model;
                }

                if (suggestionType == "FRAGMENTATION")
                {
                    this.fragmentation_parameters = model;
                }
                //
                if (suggestionType == "ADVANTAGE_SHOPPING_CAMPAIGN_FRAGMENTATION")
                {
                    this.asc_fragmentation_parameters = model;
                }
            }
        }
    }
}
