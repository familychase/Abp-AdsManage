namespace Ads.Automation.Application.Contracts.Entity.Pixel;

public class GetPixelListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 关键字筛选（模糊匹配 PixelNo 或 PixelName）
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// 按关联账户编号筛选
    /// </summary>
    public string? AccountNo { get; set; }
}
