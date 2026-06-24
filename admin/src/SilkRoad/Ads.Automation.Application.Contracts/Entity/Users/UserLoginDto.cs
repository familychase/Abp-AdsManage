using System.ComponentModel.DataAnnotations;
using Ads.Automation.Domain.Shared.Localization;

namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// User login input DTO
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// User code (login account)
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(V), ErrorMessageResourceName = nameof(V.UserCodeRequired))]
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(V), ErrorMessageResourceName = nameof(V.PasswordRequired))]
        public string Password { get; set; } = string.Empty;
    }
}
