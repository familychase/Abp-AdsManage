using System;
using System.Linq;

namespace Ads.Automation.Infrastructure.Caching.Redis
{
    public static class RedisKeyHelper
    {
        public static string Generate(params string[] parts)
        {
            if (parts == null || parts.Length == 0)
                throw new ArgumentException("At least one key part must be provided.", nameof(parts));

            return string.Join(':', parts.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()));
        }
    }
}
