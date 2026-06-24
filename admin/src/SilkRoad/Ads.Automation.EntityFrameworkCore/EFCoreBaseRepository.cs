using Ads.Automation.Domain.Shared.Common;
using Ads.Automation.Infrastructure.Repository;
using Volo.Abp.Domain.Entities;

namespace Ads.Automation.EntityFrameworkCore;

public class EFCoreBaseRepository<TEntity> : EfCoreRepository<AdsAutomationDbContext, TEntity, long>, IBaseRepository<TEntity>
    where TEntity : class, IEntity<long>
{
    private readonly UserInfoContext _userInfo;

    public EFCoreBaseRepository(
        IDbContextProvider<AdsAutomationDbContext> dbContextProvider,
        UserInfoContext userInfo)
        : base(dbContextProvider)
    {
        _userInfo = userInfo;
    }

    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        SetCreatorId(entity);
        return await base.InsertAsync(entity, autoSave, cancellationToken);
    }

    public override async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            SetCreatorId(entity);
        }
        await base.InsertManyAsync(entities, autoSave, cancellationToken);
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        SetLastModifierId(entity);
        return await base.UpdateAsync(entity, autoSave, cancellationToken);
    }

    public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            SetLastModifierId(entity);
        }
        await base.UpdateManyAsync(entities, autoSave, cancellationToken);
    }

    public override async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        SetDeletionAudit(entity);
        await base.DeleteAsync(entity, autoSave, cancellationToken);
    }

    public override async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            SetDeletionAudit(entity);
        }
        await base.DeleteManyAsync(entities, autoSave, cancellationToken);
    }

    #region 审计字段赋值

    /// <summary>
    /// 设置创建者ID（仅当实体实现 IHasCreationTimeEntity 且 CreatorId 未设置时生效）
    /// </summary>
    private void SetCreatorId(TEntity entity)
    {
        if (entity is IHasCreationTimeEntity creation && creation.CreatorId == 0)
        {
            creation.CreatorId = _userInfo.UserId;
        }
    }

    /// <summary>
    /// 设置最后修改者ID（仅当实体实现 IHasModificationTimeEntity 时生效）
    /// </summary>
    private void SetLastModifierId(TEntity entity)
    {
        if (entity is IHasModificationTimeEntity modification)
        {
            modification.LastModifierId = _userInfo.UserId > 0 ? _userInfo.UserId : null;
        }
    }

    /// <summary>
    /// 设置删除审计信息（仅当实体实现 ISoftDeleteEntity 时生效）
    /// </summary>
    private void SetDeletionAudit(TEntity entity)
    {
        if (entity is ISoftDeleteEntity softDelete)
        {
            softDelete.DeleterId = _userInfo.UserId > 0 ? _userInfo.UserId : null;
            softDelete.DeletionTime = DateTime.Now;
        }
    }

    #endregion
}
