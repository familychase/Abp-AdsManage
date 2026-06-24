using Volo.Abp.Domain.Entities;

namespace Ads.Automation.Infrastructure.Repository
{
    /// <summary>
    /// 聚合根基类，用于统一管理实体主键类型
    /// 如果需要修改主键类型，只需在此类中修改，所有继承该类的实体都会自动适配
    /// </summary>
    /// <typeparam name="TKey">主键类型，默认为long</typeparam>
    public abstract class AggregateRootEntity<TKey> : Entity<TKey>
        where TKey : notnull
    {
        protected AggregateRootEntity()
        {
        }

        protected AggregateRootEntity(TKey id) : base(id)
        {
        }
    }

    /// <summary>
    /// 默认主键为long的聚合根基类
    /// 如果主键类型需要变更为其他类型（如string、Guid等），
    /// 所有实体只需要修改继承的基类，不需要修改实体本身
    /// </summary>
    public abstract class AggregateRootEntity : AggregateRootEntity<long>
    {
        protected AggregateRootEntity()
        {
        }

        protected AggregateRootEntity(long id) : base(id)
        {
        }
    }
}
