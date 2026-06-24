/*
 *@author: huangk 2022-12-22 15:59:32
 */
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{
    public partial class MetaInput
    {
        /// <summary>
        /// 获取复制缓存信息 请求参数
        /// </summary>
        public class CopyDto
        {
            public List<Asyncbatch> asyncbatch { get; set; } = null!;

        }

        public class Asyncbatch
        {
            /// <summary>
            /// 
            /// </summary>
            public string method { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string relative_url { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string body { get; set; } = null!;
        }

    }
}
