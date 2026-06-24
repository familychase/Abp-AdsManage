namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 广告复制应用服务接口
/// </summary>
public interface IDuplicateAppService : IApplicationService
{
    /// <summary>
    /// 账户内复制
    /// </summary>
    Task<AdsDuplicateLoggingDto> InternalDuplicateAsync(InternalDuplicateInput input);

    /// <summary>
    /// 跨账户复制
    /// </summary>
    Task<AdsDuplicateLoggingDto> ExternalDuplicateAsync(ExternalDuplicateInput input);

    /// <summary>
    /// 广告系列复制提交（校验广告系列编号有效性后创建待执行记录）
    /// </summary>
    Task<AdsDuplicateLoggingDto> SubmitCampaignAsync(CampaignSubmitInput input);

    /// <summary>
    /// 广告组复制提交（校验广告组编号有效性后创建待执行记录）
    /// </summary>
    Task<AdsDuplicateLoggingDto> SubmitAdSetAsync(AdSetSubmitInput input);

    /// <summary>
    /// 获取广告系列复制日志列表（分页）
    /// </summary>
    Task<PagedResultDto<AdsDuplicateLoggingDto>> GetCampaignListAsync(GetDuplicateLoggingListInput input);

    /// <summary>
    /// 获取广告组复制日志列表（分页）
    /// </summary>
    Task<PagedResultDto<AdsDuplicateLoggingDto>> GetAdSetListAsync(GetDuplicateLoggingListInput input);

    /// <summary>
    /// 获取复制明细列表（按日志ID查询全部明细）
    /// </summary>
    Task<List<AdsDuplicateDetailDto>> GetDetailListAsync(long logId);
}
