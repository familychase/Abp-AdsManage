/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {

        /// <summary>
        /// 应用信息
        /// </summary>
        public class AppDetail
        {
            /// <summary>
            /// 应用ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 应用名称
            /// </summary>
            public string name { get; set; } = null!;

        }


        /// <summary>
        /// 应用详细信息
        /// </summary>
        public class AppInfo
        {
            /// <summary>
            /// 应用id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 应用名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 应用程序图标的 URL
            /// </summary>
            public string? icon_url { get; set; } 

            /// <summary>
            /// 应用程序的移动商店 URL
            /// </summary>
            public StoreUrls? object_store_urls { get; set; } 

            /// <summary>
            /// 平台类型
            /// </summary>
            public string[]? supported_platforms { get; set; }
        }


        /// <summary>
        /// 应用程序的移动商店URL模型
        /// </summary>
        public class StoreUrls
        {
            /// <summary>
            /// Google Play 商店中应用的 URL，默认安卓
            /// </summary>
            public string? google_play { get; set; } 

            /// <summary>
            /// iTunes 商店中应用程序的 URL，默认苹果
            /// </summary>
            public string? itunes { get; set; } 

            /// <summary>
            /// iTunes 商店中 iPad 应用程序的 URL，默认苹果
            /// </summary>
            public string? itunes_ipad { get; set; }


        }
    }
}
