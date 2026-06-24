namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 批量创建字典类 + 字典项 DTO
/// </summary>
public class CreateUpdateDictSortWithItemsDto
{
    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public SysDictSortType DictSortType { get; set; }

    /// <summary>
    /// 字典类编码
    /// </summary>
    public string DictSortCode { get; set; } = string.Empty;

    /// <summary>
    /// 字典类名称
    /// </summary>
    public string DictSortName { get; set; } = string.Empty;

    /// <summary>
    /// 备注信息
    /// </summary>
    public string Remarks { get; set; } = string.Empty;

    /// <summary>
    /// 字典项列表
    /// </summary>
    public List<CreateUpdateSysDictItemDto> DictItems { get; set; } = new();
}
