using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Ads.Automation.Infrastructure.Repository
{
    /// <summary>
    /// 通用Repository接口，为继承AggregateRoot的实体提供默认主键类型为long的Repository接口
    /// 这样在注入时，只需声明IRepository&lt;TEntity&gt;，而不需要显式写出IRepository&lt;TEntity, long&gt;
    /// </summary>
    /// <typeparam name="TEntity">实体类型，应继承自AggregateRoot</typeparam>
    public interface IBaseRepository<TEntity> : Volo.Abp.Domain.Repositories.IRepository<TEntity, long>
        where TEntity : class, IEntity<long>
    {
    }
}
