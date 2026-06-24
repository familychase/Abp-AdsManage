/*
 *@author: huangk  2022-12-8 11:40:58
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {

        #region 广告管理（广告系列、广告组）复制

        /// <summary>
        /// 异步复制缓存信息
        /// </summary>
        public class AsyncSessions
        {
            /// <summary>
            /// 异步缓存信息列表
            /// </summary>
            public List<AsyncSessionInfo> async_sessions { get; set; } = new List<AsyncSessionInfo>();

            /// <summary>
            /// 异步缓存信息
            /// </summary>
            public class AsyncSessionInfo
            {
                /// <summary>
                /// 
                /// </summary>
                public string id { get; set; } = null!;

                /// <summary>
                /// 名称
                /// </summary>
                public string? name { get; set; }
            }
        }

        /// <summary>
        /// 异步复制缓存信息结果
        /// </summary>
        public class AsyncSessionResult
        {
            /// <summary>
            /// 结果
            /// </summary>
            public string? result { get; set; }

            /// <summary>
            /// 缓存id
            /// </summary>
            public string id { get; set; } = null!;

        }
        #endregion
    }

}
