
/*
 * @author: zhangwenjie 2023-02-20 17:41
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 模板广告创意
    /// </summary>
    public sealed class MetaTemplateAdCreativeDto
    {
        /// <summary>
        /// 模板广告的深度链接网站
        /// </summary>
        public MetaTemplateAdCreativeUrlSpecDto web { get; set; } = null!;
    }
}
