namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典类 DTO
/// </summary>
public class SysDictSortDto
{
    public long Id { get; set; }
    public PlatformType Platform { get; set; }
    public SysDictSortType DictSortType { get; set; }
    public string DictSortCode { get; set; } = string.Empty;
    public string DictSortName { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string CreationTime { get; set; } = string.Empty;
}
