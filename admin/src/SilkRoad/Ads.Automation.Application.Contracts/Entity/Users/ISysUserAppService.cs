namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// 用户信息Interface
    /// </summary>
    public interface ISysUserAppService: IApplicationService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SysUserDto> GetAsync(long id);
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<SysUserDto>> GetListAsync(GetSysUserListInput input);
        /// <summary>
        /// 创建用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SysUserDto> CreateAsync(CreateUpdateSysUserDto input);
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SysUserDto> UpdateAsync(long id, CreateUpdateSysUserDto input);
        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SysUserDto> UpdateSelfAsync(long id, UpdateSysUserSelfDto input);
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(long id);
        /// <summary>
        /// 重置秘密
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ResetPasswordAsync(long id);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> ChangePasswordAsync(long id, ChangeSysUserPasswordDto input);

        /// <summary>
        /// 获取当前登录用户自信息（含用户信息、权限标识、菜单树）
        /// </summary>
        /// <param name="userId">当前用户ID</param>
        /// <returns></returns>
        Task<UserSelfInfoDto> GetSelfInfoAsync(long userId);
    }
}
