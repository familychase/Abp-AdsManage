using Volo.Abp.Domain.Entities;

namespace Ads.Automation.Domain.Pixel;

/// <summary>
/// 账户-像素关联实体（多对多关系）
/// 一个像素可以被多个账户使用，一个账户可以有多个像素
/// </summary>
public class AdsAccountPixel : Entity<long>
{
    /// <summary>
    /// 广告账户编号（Meta account_id）
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 像素 ID（AdsPixel.Id）
    /// </summary>
    public long PixelId { get; set; }

    /// <summary>
    /// Meta 像素编号（冗余字段，方便调试和查询）
    /// </summary>
    public string PixelNo { get; set; } = string.Empty;
}
