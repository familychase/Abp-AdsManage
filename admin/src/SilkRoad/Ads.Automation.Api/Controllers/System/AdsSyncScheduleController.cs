using System.Text.Json;
using Ads.Automation.Application.Contracts.Entity.SyncSchedule;
using Ads.Automation.Application.Contracts.IntegrationJobs;
using Ads.Automation.Domain.SyncSchedule;
using Ads.Automation.Infrastructure.RabbitMq.Interfaces;
using Ads.Automation.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Linq;

namespace Ads.Automation.Api.Controllers.System;

/// <summary>
/// 同步调度计划控制器
/// </summary>
[Route("api/system/sync_schedule")]
[ApiController]
public class AdsSyncScheduleController : ApiControllerBase
{
    private readonly IAdsSyncScheduleAppService _scheduleAppService;
    private readonly IBaseRepository<AdsSyncSchedule> _scheduleRepository;
    private readonly IAsyncQueryableExecuter _asyncExecuter;
    private readonly IJobQueue _jobQueue;

    /// <summary>默认间隔（当 ExtendingData 为空或解析失败时使用）</summary>
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromHours(1);

    public AdsSyncScheduleController(
        IAdsSyncScheduleAppService scheduleAppService,
        IBaseRepository<AdsSyncSchedule> scheduleRepository,
        IAsyncQueryableExecuter asyncExecuter,
        IJobQueue jobQueue)
    {
        _scheduleAppService = scheduleAppService;
        _scheduleRepository = scheduleRepository;
        _asyncExecuter = asyncExecuter;
        _jobQueue = jobQueue;
    }

    /// <summary>
    /// 获取调度计划
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        return Success(await _scheduleAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取调度计划列表
    /// </summary>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetAdsSyncScheduleListInput input)
    {
        return Success(await _scheduleAppService.GetListAsync(input));
    }

    /// <summary>
    /// 创建调度计划
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateAdsSyncScheduleDto input)
    {
        return Success(await _scheduleAppService.CreateAsync(input));
    }

    /// <summary>
    /// 修改调度计划
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CreateUpdateAdsSyncScheduleDto input)
    {
        return Success(await _scheduleAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除调度计划
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _scheduleAppService.DeleteAsync(id);
        return Success<object?>(null);
    }

    /// <summary>
    /// 手动推送指定调度计划的同步任务（立即执行一次）
    /// </summary>
    [HttpPost("{id}/push")]
    public async Task<IActionResult> PushAsync(long id)
    {
        var schedule = await _scheduleAppService.GetAsync(id);

        // 根据 JobName 分发到对应的 JobQueue
        switch (schedule.JobName)
        {
            case nameof(SyncAdAccountJobArgs):
                _jobQueue.Enqueue(new SyncAdAccountJobArgs { ChannelId = long.Parse(schedule.ResourceId) });
                break;

            case nameof(SyncAdPageJobArgs):
                _jobQueue.Enqueue(new SyncAdPageJobArgs { ChannelId = long.Parse(schedule.ResourceId) });
                break;

            case nameof(SyncAdPixelJobArgs):
                _jobQueue.Enqueue(new SyncAdPixelJobArgs { AccountNo = schedule.ResourceId });
                break;

            case nameof(SyncAdCampaignJobArgs):
                _jobQueue.Enqueue(new SyncAdCampaignJobArgs { AccountNo = schedule.ResourceId });
                break;

            case nameof(SyncAdReportJobArgs):
                _jobQueue.Enqueue(new SyncAdReportJobArgs
                {
                    AccountNo = schedule.ResourceId,
                    ReportDate = SyncAdReportJobArgs.Resolve(schedule.LinkDate)
                });
                break;

            case nameof(SyncAdAudienceJobArgs):
                _jobQueue.Enqueue(new SyncAdAudienceJobArgs { AccountNo = schedule.ResourceId });
                break;

            default:
                return BadRequest(new { message = $"不支持的 JobName: {schedule.JobName}" });
        }

        // 更新下次发布时间 = 现在 + 配置间隔（避免定时调度重复触发）
        var interval = ParseInterval(schedule.ExtendingData);
        var updateInput = new CreateUpdateAdsSyncScheduleDto
        {
            ActionType = schedule.ActionType,
            ResourceId = schedule.ResourceId,
            ResourceType = schedule.ResourceType,
            Platform = schedule.Platform,
            JobName = schedule.JobName,
            ExtendingData = schedule.ExtendingData,
            Level = schedule.Level,
            IsAudience = schedule.IsAudience,
            LinkDate = schedule.LinkDate,
            NextPublishTime = DateTime.Now + interval
        };
        await _scheduleAppService.UpdateAsync(id, updateInput);

        return Success<object?>(null);
    }

    /// <summary>
    /// 推送指定广告账户的全部同步任务（像素/广告结构/报表/受众包）
    /// </summary>
    /// <param name="accountNo">广告账户编号</param>
    [HttpPost("account/{accountNo}/push")]
    public async Task<IActionResult> PushAccountAsync(string accountNo)
    {
        var scheduleQuery = await _scheduleRepository.GetQueryableAsync();
        var accountJobNames = new[]
        {
            nameof(SyncAdPixelJobArgs),
            nameof(SyncAdCampaignJobArgs),
            nameof(SyncAdReportJobArgs),
            nameof(SyncAdAudienceJobArgs),
        };

        var schedules = await _asyncExecuter.ToListAsync(
            scheduleQuery.Where(s => s.ResourceId == accountNo && accountJobNames.Contains(s.JobName)));

        if (schedules.Count == 0)
            return Fail($"未找到账户 {accountNo} 的同步计划");

        foreach (var schedule in schedules)
        {
            switch (schedule.JobName)
            {
                case nameof(SyncAdPixelJobArgs):
                    _jobQueue.Enqueue(new SyncAdPixelJobArgs { AccountNo = accountNo });
                    break;

                case nameof(SyncAdCampaignJobArgs):
                    _jobQueue.Enqueue(new SyncAdCampaignJobArgs { AccountNo = accountNo });
                    break;

                case nameof(SyncAdReportJobArgs):
                    _jobQueue.Enqueue(new SyncAdReportJobArgs
                    {
                        AccountNo = accountNo,
                        ReportDate = SyncAdReportJobArgs.Resolve(schedule.LinkDate)
                    });
                    break;

                case nameof(SyncAdAudienceJobArgs):
                    _jobQueue.Enqueue(new SyncAdAudienceJobArgs { AccountNo = accountNo });
                    break;
            }

            // 更新下次发布时间
            var interval = ParseInterval(schedule.ExtendingData);
            schedule.SetNextPublishTime(DateTime.Now + interval);
            await _scheduleRepository.UpdateAsync(schedule);
        }

        return Success(new
        {
            pushedCount = schedules.Count,
            accountNo
        });
    }

    /// <summary>
    /// 从 ExtendingData JSON 中解析调度间隔。
    /// 格式：{"IntervalSeconds": 10800}
    /// </summary>
    private static TimeSpan ParseInterval(string? extendingData)
    {
        if (string.IsNullOrWhiteSpace(extendingData))
            return DefaultInterval;

        try
        {
            using var doc = JsonDocument.Parse(extendingData);
            if (doc.RootElement.TryGetProperty("IntervalSeconds", out var prop) && prop.TryGetInt32(out var seconds))
                return TimeSpan.FromSeconds(seconds);
        }
        catch (JsonException) { }

        return DefaultInterval;
    }
}
