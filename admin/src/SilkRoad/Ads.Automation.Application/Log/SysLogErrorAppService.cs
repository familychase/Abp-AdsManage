namespace Ads.Automation.Application.Log;

/// <summary>
/// 错误日志应用服务 —— API 路由层
/// 支持按 ID 倒序、消息 / 异常模糊查询
/// </summary>
public class SysLogErrorAppService : ApplicationService, ISysLogErrorAppService
{
    private readonly ISysLogErrorRepository _sysLogErrorRepository;

    public SysLogErrorAppService(ISysLogErrorRepository sysLogErrorRepository)
    {
        _sysLogErrorRepository = sysLogErrorRepository;
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<SysLogErrorDto>> GetListAsync(GetSysLogErrorListInput input)
    {
        var query = (await _sysLogErrorRepository.GetQueryableAsync()).AsNoTracking();

        // 日志级别精确匹配
        if (!string.IsNullOrWhiteSpace(input.Level))
            query = query.Where(x => x.Level == input.Level);

        // Logger 模糊匹配
        if (!string.IsNullOrWhiteSpace(input.Logger))
            query = query.Where(x => x.Logger.Contains(input.Logger));

        // 请求路径精确匹配
        if (!string.IsNullOrWhiteSpace(input.RequestPath))
            query = query.Where(x => x.RequestPath == input.RequestPath);

        // 关键字模糊匹配 Message 或 Exception
        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x =>
                x.Message.Contains(input.Keyword) ||
                (x.Exception != null && x.Exception.Contains(input.Keyword)));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Select(x => new SysLogErrorDto
            {
                Id = x.Id,
                Level = x.Level,
                Logger = x.Logger,
                Message = x.Message,
                Exception = x.Exception,
                RequestPath = x.RequestPath,
                CreationTime = x.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return new PagedResultDto<SysLogErrorDto>(totalCount, items);
    }
}
