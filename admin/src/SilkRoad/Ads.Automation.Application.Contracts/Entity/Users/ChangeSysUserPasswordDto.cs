using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Users
{
    public class ChangeSysUserPasswordDto
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required]
        public string OldPassword { get; set; } = string.Empty;
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
