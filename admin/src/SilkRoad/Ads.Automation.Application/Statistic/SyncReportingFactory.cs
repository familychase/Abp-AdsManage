using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Statistic
{
    /// <summary>
    /// 简单工厂模式 
    /// 通过 DI 容器实现的策略工厂
    /// </summary>
    public class SyncReportingFactory
    {
        private readonly Dictionary<PlatformType, ISyncReportingService> _services;

        /// <summary>
        /// ctor - DI.
        /// </summary>
        /// <param name="services"></param>
        public SyncReportingFactory(IEnumerable<ISyncReportingService> services)
        {
            _services = services.ToDictionary(
                s => s.Platform,
                s => s
            );
        }

        /// <summary>
        /// 获取指定媒体平台的抽象API服务.
        /// </summary>
        /// <param name="platform"></param>
        /// <exception cref="KeyNotFoundException">当平台不支持时抛出</exception>
        /// <returns></returns>
        public ISyncReportingService Get(PlatformType platform)
        {
            if (_services.TryGetValue(platform, out var service))
                return service;

            throw new NotSupportedException($"Platform {platform} is not supported");
        }

        /// <summary>
        /// 尝试获取服务，避免异常
        /// </summary>
        public bool TryGet(PlatformType platform, out ISyncReportingService service)
        {
            return _services.TryGetValue(platform, out service);
        }

        /// <summary>
        /// 获取所有支持的平台
        /// </summary>
        public IEnumerable<PlatformType> SupportedPlatforms => _services.Keys;
    }
}
