namespace Ads.Automation.Domain.Dict;

/// <summary>
/// 系统字典类
/// </summary>
public class SysDictSort : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity
{
    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; private set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public SysDictSortType DictSortType { get; private set; }

    /// <summary>
    /// 字典类编码
    /// </summary>
    public string DictSortCode { get; private set; } = string.Empty;

    /// <summary>
    /// 字典类名称
    /// </summary>
    public string DictSortName { get; private set; } = string.Empty;

    /// <summary>
    /// 备注信息
    /// </summary>
    public string Remarks { get; private set; } = string.Empty;

    /// <summary>
    /// 字典项列表
    /// </summary>
    public IList<SysDictItem>? Items { get; private set; }

    // ===== 审计字段 =====

    public long CreatorId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public long? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }

    // ===== 构造函数 =====

    private SysDictSort() { }

    internal SysDictSort(
        long id,
        PlatformType platform,
        SysDictSortType dictSortType,
        string dictSortCode,
        string dictSortName,
        string remarks,
        long creatorId = 0)
        : base(id)
    {
        Platform = platform;
        DictSortType = dictSortType;
        DictSortCode = dictSortCode;
        DictSortName = dictSortName;
        Remarks = remarks;
        CreatorId = creatorId;
        LastModificationTime = DateTime.Now;
        LastModifierId = creatorId;
    }

    /// <summary>
    /// 创建系统字典类
    /// </summary>
    public static SysDictSort Create(
        PlatformType platform,
        SysDictSortType dictSortType,
        string dictSortCode,
        string dictSortName,
        string remarks = "",
        long creatorId = 0)
    {
        return new SysDictSort(
            IdGenerator.GetNextId(),
            platform,
            dictSortType,
            dictSortCode,
            dictSortName,
            remarks,
            creatorId);
    }

    // ===== 更新方法 =====

    public void SetPlatform(PlatformType platform) => Platform = platform;
    public void SetDictSortType(SysDictSortType dictSortType) => DictSortType = dictSortType;
    public void SetDictSortCode(string dictSortCode) => DictSortCode = dictSortCode;
    public void SetDictSortName(string dictSortName) => DictSortName = dictSortName;
    public void SetRemarks(string remarks) => Remarks = remarks;
}
