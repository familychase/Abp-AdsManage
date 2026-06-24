namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// 获取用户信息列表Input
    /// </summary>
    public class GetSysUserListInput : BasePagedAndSortedRequestDto
    {
        /// <summary>
        /// 关键字过滤（账号/姓名/手机号）
        /// </summary>
        public string? FilterText { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DepartmentId { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// 状态过滤
        /// </summary>
        public UserStatusType? Status { get; set; }
    }
}
