namespace Ads.Automation.Application.Contracts.Log;

/// <summary>
/// 错误日志应用服务接口
/// </summary>
public interface ISysLogErrorAppService : IApplicationService
{
    /// <summary>
    /// 分页查询错误日志（按 ID 倒序，支持消息 / 异常模糊匹配）
    /// </summary>
    Task<PagedResultDto<SysLogErrorDto>> GetListAsync(GetSysLogErrorListInput input);
}
