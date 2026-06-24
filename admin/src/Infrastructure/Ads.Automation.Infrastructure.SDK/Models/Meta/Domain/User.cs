/*
 * @author: hujingtian 2022-12-7 11:29:54
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 获取邮箱实体
        /// </summary>
        public class User
        {
            /// <summary>
            /// 用户Id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 用户名称
            /// </summary>
            public string? name { get; set; } = null!;


            /// <summary>
            /// 邮箱
            /// </summary>
            public string? email { get; set; } = null!;

            /// <summary>
            /// BM信息列表
            /// </summary>
            public Businesses? businesses { get; set; }

        }

        /// <summary>
        /// 
        /// </summary>
        public class Businesses : MetaPagedDto<BmInfo>
        {

        }

        /// <summary>
        ///  BM信息
        /// </summary>
        public class BmInfo
        {
            /// <summary>
            /// Bm的编号
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// Bm的名称
            /// </summary>
            public string name { get; set; } = null!;

        }
    }
}
