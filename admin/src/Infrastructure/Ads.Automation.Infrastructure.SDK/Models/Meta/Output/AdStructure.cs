/*
 * @author huangk  2022-12-7 17:57:47
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{

    public partial class MetaOutput
    {
        /// <summary>
        /// 广告结构对象，只有id,name
        /// </summary>
        public class AdStructure        
        {
            /// <summary>
            /// id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 名称
            /// </summary>
            public string name { get; set; }=null!;
        }
    }
}
