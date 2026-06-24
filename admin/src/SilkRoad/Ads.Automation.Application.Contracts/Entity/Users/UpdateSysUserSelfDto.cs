using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Users
{
    public class UpdateSysUserSelfDto
    {
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
    }
}
