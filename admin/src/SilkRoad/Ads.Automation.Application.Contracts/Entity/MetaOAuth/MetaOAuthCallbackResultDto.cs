namespace Ads.Automation.Application.Contracts.Entity.MetaOAuth
{
    /// <summary>
    /// Meta OAuth 授权回调结果
    /// </summary>
    public class MetaOAuthCallbackResultDto
    {
        /// <summary>
        /// 渠道 ID
        /// </summary>
        public long ChannelId { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; } = default!;

        /// <summary>
        /// 媒体平台
        /// </summary>
        public string Platform { get; set; } = default!;

        /// <summary>
        /// 授权状态
        /// </summary>
        public string ChannelState { get; set; } = default!;

        /// <summary>
        /// Meta 用户 ID
        /// </summary>
        public string MediaUserId { get; set; } = default!;

        /// <summary>
        /// Token 过期时间
        /// </summary>
        public DateTime? Expired { get; set; }

        /// <summary>
        /// 是否为新创建的渠道（false 表示更新已有渠道）
        /// </summary>
        public bool IsNew { get; set; }
    }
}
