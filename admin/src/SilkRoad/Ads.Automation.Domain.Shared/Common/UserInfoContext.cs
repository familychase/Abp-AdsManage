using Ads.Automation.Domain.Shared.Enums;

namespace Ads.Automation.Domain.Shared.Common;

/// <summary>
/// 当前登录用户上下文（Scoped），存放从 Token 解析出的用户信息
/// 通过构造函数注入到控制器或服务中使用
/// </summary>
public class UserInfoContext
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户账号
    /// </summary>
    public string UserCode { get; set; } = string.Empty;

    /// <summary>
    /// 用户姓名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 完整用户信息 DTO（来自 Redis 缓存）
    /// </summary>
    public UserInfoDto? UserInfo { get; set; }

    /// <summary>
    /// 是否已登录（是否包含有效用户信息）
    /// </summary>
    public bool IsAuthenticated => UserInfo != null;
}

/// <summary>
/// 用户信息Dto
/// </summary>
public class UserInfoDto
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
    public string? Email { get; private set; } = string.Empty;
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
