
using Volo.Abp.Auditing;

namespace Ads.Automation.Infrastructure.Repository
{
    public interface IHasCreationTimeEntity : IHasCreationTime
    {
        /// <summary>
        /// 创建人
        /// </summary>
        long CreatorId { get; set; }
    }
}
