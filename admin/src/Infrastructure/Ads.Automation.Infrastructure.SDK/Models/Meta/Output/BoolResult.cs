/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{
    public partial class MetaOutput
    {
        /// <summary>
        /// 更新，删除——结果辅助类
        /// </summary>
        public class BoolResult
        {
            /// <summary>
            /// 是否请求成功
            /// </summary>
            public bool success { get; set; }
        }
    }
}
