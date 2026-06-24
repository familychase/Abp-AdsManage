using Ads.Automation.Application.Contracts.Entity.Menus;

namespace Ads.Automation.Application.Contracts.Entity.Users;

/// <summary>
/// 当前登录用户自信息Dto（含用户信息、权限标识、菜单树）
/// </summary>
public class UserSelfInfoDto
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    public SysUserDto UserInfo { get; set; } = null!;

    /// <summary>
    /// 权限标识列表（如 system:user:list）
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// 菜单树
    /// </summary>
    public List<MenuTreeNodeDto> Menus { get; set; } = new();
}
