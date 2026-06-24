using Ads.Automation.Domain.Shared.Localization;
using Microsoft.Extensions.Localization;

namespace Ads.Automation.Application.Entity.Users
{
    public class SysUserLoginService : ApplicationService, ISysUserLoginService
    {
        private readonly ISysUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IStringLocalizer<AdsAutomationResource> _l;

        private static readonly TimeSpan TokenExpiry = TimeSpan.FromDays(1);

        public SysUserLoginService(
            ISysUserRepository userRepository,
            ICacheService cacheService,
            IStringLocalizer<AdsAutomationResource> localizer)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _l = localizer;
        }

        public async Task<UserLoginResultDto> LoginAsync(UserLoginDto input)
        {
            var user = await _userRepository.FindByCodeAsync(input.UserCode);
            if (user == null)
                throw new Domain.Shared.Common.BusinessException(_l["User:InvalidCredentials"], 400);

            if (!user.CheckPassword(input.Password))
                throw new Domain.Shared.Common.BusinessException(_l["User:InvalidCredentials"], 400);

            if (user.Status != UserStatusType.ACTIVE)
                throw new Domain.Shared.Common.BusinessException(_l["User:Disabled"], 400);

            var tokenValue = Guid.NewGuid().ToString("N");
            var tokenKey = $"token:{tokenValue}";

            var userDto = ObjectMapper.Map<SysUser, UserInfoDto>(user);
            await _cacheService.SetAsync(tokenKey, userDto, TokenExpiry);

            user.UpdateLastLoginTime(DateTime.Now);
            await _userRepository.UpdateAsync(user);

            return new UserLoginResultDto
            {
                Token = tokenValue,
                UserInfo = userDto
            };
        }

        public async Task LogoutAsync(string? token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var tokenKey = $"token:{token}";
                await _cacheService.RemoveAsync(tokenKey);
            }
        }
    }
}
