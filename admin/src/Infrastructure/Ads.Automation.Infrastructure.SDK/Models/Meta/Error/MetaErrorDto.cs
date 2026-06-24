/*
 * @author: zhangwenjie 2022-10-20 18:09
 */


namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Error
{
    /// <summary>
    /// Meta 平台的错误信息模型
    /// </summary>
    public class MetaErrorDto
    {
        /// <summary>
        /// 消息
        /// </summary>
        public Infomation? error { get; set; }

        public class Infomation
        {
            /// <summary>
            /// 消息
            /// </summary>
            public string message { get; set; }=string.Empty;

            /// <summary>
            /// 类型
            /// </summary>
            public string type { get; set; } = string.Empty;

            /// <summary>
            /// code
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// fbtrace_id
            /// </summary>
            public string fbtrace_id { get; set; } = string.Empty;

            /// <summary>
            /// 错误信息下级编码
            /// </summary>
            public int error_subcode { get; set; }

            /// <summary>
            /// 错误信息标题
            /// </summary>
            public string error_user_title { get; set; } = string.Empty;

            /// <summary>
            /// 错误信息内容
            /// </summary>
            public string error_user_msg { get; set; } = string.Empty;
        }
    }
}
