namespace Ads.Automation.Domain.Pixel;

/// <summary>
/// Meta 广告像素实体（纯像素信息，关联关系见 AdsAccountPixel）
/// </summary>
public class AdsPixel : AggregateRootEntity
{
    /// <summary>
    /// Meta 像素编号
    /// </summary>
    public string PixelNo { get; private set; } = string.Empty;

    /// <summary>
    /// 像素名称
    /// </summary>
    public string PixelName { get; private set; } = string.Empty;

    /// <summary>
    /// 像素追踪代码（JS）
    /// </summary>
    public string? Code { get; private set; }

    /// <summary>
    /// 最后同步时间
    /// </summary>
    public DateTime LastSyncTime { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    // ===== 构造函数 =====

    private AdsPixel() { }

    public static AdsPixel Create(
        string pixelNo,
        string pixelName,
        string? code)
    {
        return new AdsPixel(
            IdGenerator.GetNextId(),
            pixelNo,
            pixelName,
            code,
            DateTime.Now,
            DateTime.Now);
    }

    internal AdsPixel(
        long id,
        string pixelNo,
        string pixelName,
        string? code,
        DateTime lastSyncTime,
        DateTime creationTime)
        : base(id)
    {
        PixelNo = pixelNo;
        PixelName = pixelName;
        Code = code;
        LastSyncTime = lastSyncTime;
        CreationTime = creationTime;
    }

    // ===== 更新方法 =====

    public void SetPixelName(string name) => PixelName = name;
    public void SetCode(string? code) => Code = code;
    public void SetLastSyncTime(DateTime time) => LastSyncTime = time;
}
