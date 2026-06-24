
using Volo.Abp;

namespace Ads.Automation.Infrastructure.Repository
{
    public interface ISoftDeleteEntity : ISoftDelete
    {
        long? DeleterId { get; set; }

        DateTime? DeletionTime { get; set; }
    }
}
