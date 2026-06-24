/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 目录/系列结果
        /// </summary>
        public class ProductCatalogs
        {
            /// <summary>
            /// 目录/系列的no
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 目录/系列的名称
            /// </summary>
            public string name { get; set; } = null!;
            /// <summary>
            /// 目录类型
            /// </summary>
            public string? vertical { get; set; } 

            /// <summary>
            /// 系列产品Id
            /// </summary>
            public string? filter { get; set; } 

            /// <summary>
            /// 产品个数
            /// </summary>
            public int product_count { get; set; }
        }
    }
}
