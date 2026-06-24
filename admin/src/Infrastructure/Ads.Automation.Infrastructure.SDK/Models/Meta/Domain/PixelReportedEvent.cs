
/*
 * @author: zhangwenjie 2023-08-22 17:14
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 像素上报数据
        /// </summary>
        public class PixelReportedData
        {
            /// <summary>
            /// 数据
            /// </summary>
            public List<Event> data { get; set; } = null!;

            /// <summary>
            /// 像素上报事件
            /// </summary>
            public class Event
            {
                /// <summary>
                /// 事件名称
                /// </summary>
                public string? event_name { get; set; }

                /// <summary>
                /// 事件时间
                /// </summary>
                public string? event_time { get; set; }

                /// <summary>
                /// 事件发生的浏览器网址
                /// </summary>
                public string? event_source_url { get; set; }

                /// <summary>
                /// 此编号可以是广告主选定的任何唯一字符串
                /// </summary>
                public string? event_id { get; set; }

                /// <summary>
                /// 您可通过此字段指定转化发生的位置
                /// </summary>
                public string? action_source { get; set; }

                /// <summary>
                /// 包含客户信息数据的映射
                /// </summary>
                public UserData user_data { get; set; } = null!;

                /// <summary>
                /// 用户数据
                /// </summary>
                public class UserData
                {
                    /// <summary>
                    /// 与事件对应的浏览器 IP 地址必须是有效的 IPV4 或 IPV6 地址
                    /// </summary>
                    public string? client_ip_address { get; set; }

                    /// <summary>
                    /// 与事件对应的浏览器的用户代理程序
                    /// </summary>
                    public string? client_user_agent { get; set; }

                    /// <summary>
                    /// Facebook 点击编号值存储在您网域下的 _fbc 浏览器 Cookie 中
                    /// </summary>
                    public string? fbc { get; set; }

                    /// <summary>
                    /// Facebook 浏览器编号值存储在您网域下的 _fbp 浏览器 Cookie 中
                    /// </summary>
                    public string? fbp { get; set; }

                    /// <summary>
                    /// 邮箱
                    /// </summary>
                    public string? em { get; set; }

                    /// <summary>
                    /// 手机号
                    /// </summary>
                    public string? ph { get; set; }

                    /// <summary>
                    /// 您的移动广告客户编号、Android 设备中的广告编号或 Apple 设备中的广告 ID (IDFA)
                    /// </summary>
                    public string? madid { get; set; }
                }

                /// <summary>
                /// 包含事件其他企业数据的映射
                /// </summary>
                public CustomData custom_data { get; set; } = null!;

                /// <summary>
                /// 自定义其它事件
                /// </summary>
                public class CustomData
                {
                    /// <summary>
                    /// 货币
                    /// </summary>
                    public string? currency { get; set; }

                    /// <summary>
                    /// 金额
                    /// </summary>
                    public string? value { get; set; }
                }
            }
        }
    }
}
