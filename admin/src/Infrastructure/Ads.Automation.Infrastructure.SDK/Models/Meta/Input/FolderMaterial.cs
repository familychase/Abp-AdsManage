
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{
    public partial class MetaInput
    {

        public class MaterialQuery
        {
            /// <summary>
            /// BM编号
            /// </summary>
            public string? businessNo { get; set; }

            /// <summary>
            /// 文件夹Id
            /// </summary>
            public string? creative_folder_id { get; set; }

            /// <summary>
            /// 素材编号
            /// </summary>
            public string? materialNo { get; set; }

            /// <summary>
            /// 页面大小
            /// </summary>
            public int limit { get; set; }

            /// <summary>
            /// 字段列
            /// </summary>
            public string? fields { get; set; }

            /// <summary>
            /// 上一页
            /// </summary>
            public string? before { get; set; }

            /// <summary>
            /// 下一页
            /// </summary>
            public string? after { get; set; }

        }
        public class FolderMaterial
        {
            /// <summary>
            /// 图片信息
            /// </summary>
            public string? bytes { get; set; }

            /// <summary>
            /// 图片名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 视频地址
            /// </summary>
            public string? file_url { get; set; }

            /// <summary>
            /// 视频标题
            /// </summary>
            public string? title { get; set; }

            /// <summary>
            /// 文件夹Id
            /// </summary>
            public string? creative_folder_id { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FolderParameter {

            /// <summary>
            /// 文件夹名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 父级文件夹编号
            /// </summary>
            public string? parent_folder_id { get; set; }

        }

    }
}
