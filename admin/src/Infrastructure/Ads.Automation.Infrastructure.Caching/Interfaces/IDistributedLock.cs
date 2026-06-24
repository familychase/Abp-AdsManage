namespace Ads.Automation.Infrastructure.Caching.Interfaces
{
    /// <summary>
    /// 分布式锁接口，提供获取和释放分布式锁的方法，确保在分布式环境中对共享资源的访问进行同步控制。
    /// </summary>
    public interface IDistributedLock
    {
        /// <summary>
        /// 尝试获取分布式锁，返回是否获取成功
        /// </summary>
        Task<bool> AcquireAsync(string key, TimeSpan expiration);

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        Task ReleaseAsync(string key);

        /// <summary>
        /// 获取分布式锁并返回可释放的锁句柄，支持 using 模式安全释放
        /// </summary>
        Task<IDisposable?> AcquireWithHandleAsync(string key, TimeSpan expiration);
    }
}
