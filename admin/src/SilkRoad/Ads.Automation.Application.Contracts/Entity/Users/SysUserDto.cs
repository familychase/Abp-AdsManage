using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// 用户信息Dto
    /// </summary>
    public class SysUserDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 用户账号（登录账号）
        /// </summary>
        public string UserCode { get; set; } = string.Empty;
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string? AliasName { get; set; } = string.Empty;
        /// <summary>
        /// 所属部门
        /// </summary>
        public long DepartmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;
        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 是否团队管理员
        /// </summary>
        public bool IsTeamAdmin { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; } 
        /// <summary>
        /// 手机号
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public UserStatusType Status { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string? LastLoginTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreationTime { get; set; } = string.Empty;
    }
}
