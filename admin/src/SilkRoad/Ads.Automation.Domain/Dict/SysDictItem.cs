namespace Ads.Automation.Domain.Dict;

/// <summary>
/// 系统字典项
/// </summary>
public class SysDictItem : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity
{
    /// <summary>
    /// 字典类Id
    /// </summary>
    public long DictSortId { get; private set; }

    /// <summary>
    /// 父级字典Id
    /// </summary>
    public long ParentId { get; private set; }

    /// <summary>
    /// 字典项编码
    /// </summary>
    public string DictItemCode { get; private set; } = string.Empty;

    /// <summary>
    /// 字典项名称
    /// </summary>
    public string DictItemName { get; private set; } = string.Empty;

    /// <summary>
    /// 字典项名称（英文）
    /// </summary>
    public string DictItemNameEN { get; private set; } = string.Empty;

    /// <summary>
    /// 字典项值
    /// </summary>
    public string DictItemValue { get; private set; } = string.Empty;

    /// <summary>
    /// 备注信息
    /// </summary>
    public string? Remarks { get; private set; } 

    /// <summary>
    /// 序号
    /// </summary>
    public int Ordinal { get; private set; }

    /// <summary>
    /// 值类型（单位）
    /// </summary>
    public DictItemValueType ItemType { get; private set; }

    /// <summary>
    /// 环境：true 为正式环境，false 为预发布/测试环境
    /// </summary>
    public bool IsProduction { get; private set; }

    /// <summary>
    /// 字典类
    /// </summary>
    public SysDictSort? DictSort { get; private set; }

    // ===== 审计字段 =====

    public long CreatorId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public long? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }

    // ===== 构造函数 =====

    private SysDictItem() { }

    internal SysDictItem(
        long id,
        long dictSortId,
        string dictItemCode,
        string dictItemName,
        string dictItemNameEN,
        string dictItemValue,
        string remarks,
        int ordinal,
        DictItemValueType itemType,
        bool isProduction,
        long parentId = 0,
        long creatorId = 0)
        : base(id)
    {
        DictSortId = dictSortId;
        DictItemCode = dictItemCode;
        DictItemName = dictItemName;
        DictItemNameEN = dictItemNameEN;
        DictItemValue = dictItemValue;
        Remarks = remarks;
        Ordinal = ordinal;
        ItemType = itemType;
        IsProduction = isProduction;
        ParentId = parentId;
        CreatorId = creatorId;
        LastModificationTime = DateTime.Now;
        LastModifierId = creatorId;
    }

    /// <summary>
    /// 创建系统字典项
    /// </summary>
    public static SysDictItem Create(
        long dictSortId,
        string dictItemCode,
        string dictItemName,
        string dictItemNameEN,
        string dictItemValue,
        string remarks = "",
        int ordinal = 0,
        DictItemValueType itemType = DictItemValueType.NONE,
        bool isProduction = true,
        long parentId = 0,
        long creatorId = 0)
    {
        return new SysDictItem(
            IdGenerator.GetNextId(),
            dictSortId,
            dictItemCode,
            dictItemName,
            dictItemNameEN,
            dictItemValue,
            remarks,
            ordinal,
            itemType,
            isProduction,
            parentId,
            creatorId);
    }

    // ===== 更新方法 =====

    public void SetDictSortId(long dictSortId) => DictSortId = dictSortId;
    public void SetParentId(long parentId) => ParentId = parentId;
    public void SetDictItemCode(string dictItemCode) => DictItemCode = dictItemCode;
    public void SetDictItemName(string dictItemName) => DictItemName = dictItemName;
    public void SetDictItemNameEN(string dictItemNameEN) => DictItemNameEN = dictItemNameEN;
    public void SetDictItemValue(string dictItemValue) => DictItemValue = dictItemValue;
    public void SetRemarks(string remarks) => Remarks = remarks;
    public void SetOrdinal(int ordinal) => Ordinal = ordinal;
    public void SetItemType(DictItemValueType itemType) => ItemType = itemType;
    public void SetIsProduction(bool isProduction) => IsProduction = isProduction;
}
