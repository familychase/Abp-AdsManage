
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Internal
{
    /// <summary>
    /// Meta - 地理位置信息.
    /// </summary>
    public class MetaGeoLocationsDto
    {
        /// <summary>
        /// 地区人群
        /// </summary>
        public List<string>? location_types { get; set; }

        /// <summary>
        /// 国家组
        /// </summary>
        public List<string>? country_groups { get; set; }

        /// <summary>
        /// 国家，必填
        /// </summary>
        public List<string>? countries { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public List<MetaRegionCityDto>? regions { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public List<MetaRegionCityDto>? cities { get; set; }
    }
}
