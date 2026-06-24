using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Domain.Shared.Enums
{
    /// <summary>
    /// 权限类型
    /// </summary>
    public enum PermissionCodeType
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        QUERY_LIST = 1,
        /// <summary>
        /// 新增
        /// </summary>
        ADDITION = 2,
        /// <summary>
        /// 编辑
        /// </summary>
        EDITOR = 3,
        /// <summary>
        /// 删除
        /// </summary>
        DELETE = 4
    }
}
