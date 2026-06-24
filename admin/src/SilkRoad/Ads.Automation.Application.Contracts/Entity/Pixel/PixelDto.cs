namespace Ads.Automation.Application.Contracts.Entity.Pixel;

public class PixelDto
{
    /// <summary>
    /// 像素 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Meta 像素编号
    /// </summary>
    public string PixelNo { get; set; } = string.Empty;

    /// <summary>
    /// 像素名称
    /// </summary>
    public string PixelName { get; set; } = string.Empty;

    /// <summary>
    /// 像素追踪代码（JS）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 关联账户数量
    /// </summary>
    public int AccountCount { get; set; }

    /// <summary>
    /// 关联的账户编号列表
    /// </summary>
    public List<string> AssociatedAccounts { get; set; } = new();

    /// <summary>
    /// 最后同步时间
    /// </summary>
    public string? LastSyncTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreationTime { get; set; } = string.Empty;
}
