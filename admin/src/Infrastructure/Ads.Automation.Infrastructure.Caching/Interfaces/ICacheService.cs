namespace Ads.Automation.Infrastructure.Caching.Interfaces
{
    /// <summary>
    /// Redis cache 核心服务接口，提供基本的缓存操作方法，如获取、设置、删除和检查缓存项的存在性。
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取缓存信息
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// 设置缓存信息
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// 删除缓存信息
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// 批量删除缓存信息
        /// </summary>
        Task RemoveAllAsync(IEnumerable<string> keys);

        /// <summary>
        /// 检查缓存项是否存在
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 获取或创建缓存项（cache-aside 模式）
        /// </summary>
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>
        /// 刷新缓存项过期时间
        /// </summary>
        Task RefreshAsync(string key, TimeSpan expiration);

        /// <summary>
        /// 按模式查找所有匹配的 key
        /// </summary>
        Task<string[]> GetKeysByPatternAsync(string pattern);

        /// <summary>
        /// 按模式删除所有匹配的缓存项
        /// </summary>
        Task RemoveByPatternAsync(string pattern);
    }
}
