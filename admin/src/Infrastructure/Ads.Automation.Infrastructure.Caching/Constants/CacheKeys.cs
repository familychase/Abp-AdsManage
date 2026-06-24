
namespace Ads.Automation.Infrastructure.Caching.Constants
{
    public class CacheKeys
    {
        public static string User(Guid id) => $"user:{id}";

        public static string Token(Guid id) => $"token:{id}";
    }
}
