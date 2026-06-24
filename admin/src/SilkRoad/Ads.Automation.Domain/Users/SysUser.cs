using System.Security.Cryptography;
using System.Text;
using Volo.Abp.Domain.Entities;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Ads.Automation.Domain.Users
{
    /// <summary>
    /// 用户信息实体
    /// </summary>
    public class SysUser : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity, ISoftDeleteEntity
    {
        /// <summary>
        /// 用户账号（登录账号）
        /// </summary>
        public string UserCode { get; private set; } = string.Empty;
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserName { get; private set; } = string.Empty;
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string? AliasName { get; private set; } = string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        public string PasswordHash { get; private set; } = string.Empty;
        /// <summary>
        /// 密码盐
        /// </summary>
        public string Salt { get; private set; } = string.Empty;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; private set; } = string.Empty;
        /// <summary>
        /// 手机号码
        /// </summary>
        public string? PhoneNumber { get; private set; } = string.Empty;
        /// <summary>
        /// 所属部门
        /// </summary>
        public long DepartmentId { get; private set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsAdmin { get; private set; }
        /// <summary>
        /// 是否团队管理员
        /// </summary>
        public bool IsTeamAdmin { get; private set; }
        /// <summary>
        /// 状态
        /// </summary>
        public UserStatusType Status { get; private set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; private set; }
        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string? LastLoginIp { get; private set; } = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; private set; } = string.Empty;

        public long CreatorId { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;

        public long? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? DeleterId { get; set; }
        public DateTime? DeletionTime { get; set; }

        public bool IsDeleted { get; set; }

        private SysUser() { }

        public static SysUser Create(long id, string name, string code, long deptId, long roleId, UserStatusType status, bool isAdmin, bool isTeamAdmin, string? aliasName = null, string? phoneNumber = null, string? email = null)
        {
            return new SysUser(id, name, code, deptId, roleId, status, isAdmin, isTeamAdmin, aliasName, phoneNumber, email);
        }

        internal SysUser(long id, string name, string code, long deptId, long roleId, UserStatusType status, bool isAdmin, bool isTeamAdmin, string? aliasName = null, string? phoneNumber = null, string? email = null)
            : base(id)
        {
            this.Salt = Guid.NewGuid().ToString("N");
            this.Status = UserStatusType.ACTIVE;

            SetPassword("123456");
            SetCode(code);
            SetName(name);
            SetContact(aliasName, phoneNumber, email);
            SetStatus(status);
            this.DepartmentId = deptId;
            this.RoleId = roleId;
            this.IsAdmin = isAdmin;
            this.IsTeamAdmin = isTeamAdmin;
        }

        internal void SetCode(string code)
        {
            UserCode = Check.NotNullOrWhiteSpace(code, nameof(code), UserConsts.MaxUserCodeLength);
        }

        /// <summary>
        /// SHA256(password + salt)
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string? password = null)
        {
            password = password.IsNullOrEmpty() ? "123456" : password;
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(
                Encoding.UTF8.GetBytes($"{password}{Salt}"));
            this.PasswordHash = Convert.ToHexString(bytes);
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(
                Encoding.UTF8.GetBytes($"{password}{Salt}"));
            var hash = Convert.ToHexString(bytes);
            return hash == PasswordHash;
        }

        public void SetName(string name)
        {
            UserName = Check.NotNullOrWhiteSpace(name, nameof(name), UserConsts.MaxUserNameLength);
        }

        public void SetContact(string? aliasName, string? phoneNumber, string? email)
        {
            AliasName = Check.Length(aliasName, nameof(aliasName), UserConsts.MaxAliasNameLength);
            PhoneNumber = Check.Length(phoneNumber, nameof(phoneNumber), UserConsts.MaxPhoneNumberLength);
            Email = Check.Length(email, nameof(email), UserConsts.MaxEmailLength);
        }

        public void SetDepartment(long deptId)
        {
            DepartmentId = deptId;
        }

        public void SetRoleId(long roleId)
        {
            RoleId = roleId;
        }

        public void SetAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }

        public void SetTeamAdmin(bool isTeamAdmin)
        {
            IsTeamAdmin = isTeamAdmin;
        }

        public void SetStatus(UserStatusType status)
        {
            status = status;
        }

        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        public void UpdateLastLoginTime(DateTime? loginTime = null)
        {
            LastLoginTime = loginTime ?? DateTime.Now;
        }
    }
}
