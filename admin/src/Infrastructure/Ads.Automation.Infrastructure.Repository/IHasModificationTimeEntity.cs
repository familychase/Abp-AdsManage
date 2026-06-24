
using Volo.Abp.Auditing;

namespace Ads.Automation.Infrastructure.Repository
{
    public interface IHasModificationTimeEntity : IHasModificationTime
    {
        long? LastModifierId { get; set; }
    }
}
