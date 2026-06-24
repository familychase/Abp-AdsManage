using System.ComponentModel.DataAnnotations;

namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典项 创建/修改 DTO
/// </summary>
public class CreateUpdateSysDictItemDto
{
    /// <summary>
    /// 字典类Id
    /// </summary>
    [Required]
    public long DictSortId { get; set; }

    /// <summary>
    /// 父级字典Id
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 字典项编码
    /// </summary>
    [Required]
    [StringLength(64)]
    public string DictItemCode { get; set; } = string.Empty;

    /// <summary>
    /// 字典项名称
    /// </summary>
    [Required]
    [StringLength(128)]
    public string DictItemName { get; set; } = string.Empty;

    /// <summary>
    /// 字典项名称（英文）
    /// </summary>
    [StringLength(256)]
    public string DictItemNameEN { get; set; } = string.Empty;

    /// <summary>
    /// 字典项值
    /// </summary>
    [Required]
    [StringLength(256)]
    public string DictItemValue { get; set; } = string.Empty;

    /// <summary>
    /// 备注信息
    /// </summary>
    [StringLength(500)]
    public string? Remarks { get; set; } 

    /// <summary>
    /// 序号
    /// </summary>
    public int Ordinal { get; set; }

    /// <summary>
    /// 值类型（单位）
    /// </summary>
    public DictItemValueType ItemType { get; set; }

    /// <summary>
    /// 环境标识
    /// </summary>
    public bool IsProduction { get; set; } = true;
}
