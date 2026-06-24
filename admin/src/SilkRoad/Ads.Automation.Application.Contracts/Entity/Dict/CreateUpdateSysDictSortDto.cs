using System.ComponentModel.DataAnnotations;

namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典类 创建/修改 DTO
/// </summary>
public class CreateUpdateSysDictSortDto
{
    /// <summary>
    /// 媒体平台
    /// </summary>
    [Required]
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    [Required]
    public SysDictSortType DictSortType { get; set; }

    /// <summary>
    /// 字典类编码
    /// </summary>
    [Required]
    [StringLength(64)]
    public string DictSortCode { get; set; } = string.Empty;

    /// <summary>
    /// 字典类名称
    /// </summary>
    [Required]
    [StringLength(128)]
    public string DictSortName { get; set; } = string.Empty;

    /// <summary>
    /// 备注信息
    /// </summary>
    [StringLength(500)]
    public string Remarks { get; set; } = string.Empty;
}
