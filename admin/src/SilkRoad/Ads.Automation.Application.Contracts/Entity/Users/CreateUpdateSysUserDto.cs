namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// 创建/修改用户信息Dto
    /// </summary>
    public class CreateUpdateSysUserDto
    {
        /// <summary>
        /// 用户账号（登录账号）
        /// </summary>
        [Required]
        [StringLength(UserConsts.MaxUserCodeLength)]
        public string UserCode { get; set; } = string.Empty;
        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required]
        [StringLength(UserConsts.MaxUserNameLength)]
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [StringLength(UserConsts.MaxAliasNameLength)]
        public string? AliasName { get; set; } = string.Empty;
        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(UserConsts.MaxEmailLength)]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(UserConsts.MaxPhoneNumberLength)]
        public string? PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// 所属部门
        /// </summary>
        public long DepartmentId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 是否团队管理员
        /// </summary>
        public bool IsTeamAdmin { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public UserStatusType Status { get; set; }
    }
}
