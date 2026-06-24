/*
 * @author: alvin 2022-10-24 11:06
 */

// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.General
{
    /// <summary>
    /// Meta - 区域城市
    /// </summary>
    public class MetaRegionCityDto
    {
        /// <summary>
        /// 区域与城市的Key值
        /// </summary>
        public string key { get; set; } = null!;

        /// <summary>
        /// 区域与城市的名称
        /// </summary>
        public string name { get; set; } = null!;

        /// <summary>
        /// 区域与城市的国家
        /// </summary>
        public string country { get; set; } = null!;

        /// <summary>
        /// 区域的名称
        /// </summary>
        public string region { get; set; } = null!;

        /// <summary>
        /// 区域编号
        /// </summary>
        public string region_id { get; set; } = null!;
    }
}
