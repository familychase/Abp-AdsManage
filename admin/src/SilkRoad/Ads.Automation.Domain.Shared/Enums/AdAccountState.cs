using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Domain.Shared.Enums
{
    /// <summary>
    /// 广告账户状态枚举
    /// </summary>
    public enum AdAccountState
    {
        /// <summary>
        /// 所有
        /// </summary>
        ALL = 0,

        /// <summary>
        /// 正常
        /// </summary>
        NORMAL = 1,

        /// <summary>
        /// 异常
        /// </summary>
        ABNORMAL = 2,

        /// <summary>
        /// 授权丢失
        /// </summary>
        MISS = 3,

        /// <summary>
        /// 其它
        /// </summary>
        OTHER = 4,

        /// <summary>
        /// 是否受限
        /// </summary>
        LIMIT = 5
    }
}
