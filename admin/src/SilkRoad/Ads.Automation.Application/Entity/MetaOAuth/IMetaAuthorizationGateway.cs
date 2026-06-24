
namespace Ads.Automation.Application.Entity.MetaOAuth
{
    /// <summary>
    /// Meta 授权网关 — 统一的 Meta API 调用入口
    /// 自动处理 Token 解密、AccessIdentity 构建，封装渠道授权的复杂性
    /// </summary>
    public interface IMetaAuthorizationGateway
    {
        /// <summary>
        /// 获取解密后的 AccessIdentity（只读，不执行 API 调用）
        /// </summary>
        /// <param name="channelId">渠道 ID</param>
        /// <returns>解密后的访问身份凭证</returns>
        Task<AccessIdentity> GetAccessIdentityAsync(long channelId);

        /// <summary>
        /// 使用渠道授权执行 Meta API 调用（委托模式）
        /// 自动解密 Token + 构建 AccessIdentity 后传入委托
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="channelId">渠道 ID</param>
        /// <param name="action">使用 AccessIdentity 执行的异步操作</param>
        /// <returns>API 调用结果</returns>
        Task<T> ExecuteAsync<T>(long channelId, Func<AccessIdentity, Task<T>> action);

        /// <summary>
        /// 验证渠道授权是否有效（通过 /me 探测，不影响状态）
        /// </summary>
        /// <param name="channelId">渠道 ID</param>
        /// <returns>true 表示授权有效</returns>
        Task<bool> IsAuthorizationActiveAsync(long channelId);

        /// <summary>
        /// 解密 Token（用于需要原始 Token 的场景，如 Token 刷新）
        /// </summary>
        /// <param name="encryptedToken">加密的 Token</param>
        /// <returns>明文 Token</returns>
        string DecryptToken(string encryptedToken);

        /// <summary>
        /// 加密 Token（用于刷新后重新加密存储）
        /// </summary>
        /// <param name="plainToken">明文 Token</param>
        /// <returns>加密的 Token</returns>
        string EncryptToken(string plainToken);
    }
}
