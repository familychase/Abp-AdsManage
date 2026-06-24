/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {

        public class BussinessAndFolder
        {
            public MetaPagedDto<Bussiness> businesses { get; set; } = null!;
        }

        /// <summary>
        /// bm和文件夹信息
        /// </summary>
        public class Bussiness
        {
            /// <summary>
            /// BM信息id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// BM信息名称
            /// </summary>
            public string name { get; set; } = null!;

            public MetaPagedDto<BmFolder> creative_folders { get; set; } = new MetaPagedDto<BmFolder>();
        }

        /// <summary>
        /// 通过bm拉下的文件夹信息
        /// </summary>
        public class BmFolder
        {
            /// <summary>
            /// 父级文件夹名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 父级文件夹id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 文件夹信息列表
            /// </summary>
            public BMSubfolders subfolders { get; set; } = new BMSubfolders();
        }

        /// <summary>
        /// 通过bm拉下的下级文件夹信息
        /// </summary>
        public class BMSubfolders : MetaPagedDto<BmFolder>
        {

        }

        /// <summary>
        /// 文件夹信息
        /// </summary>
        public class Folder
        {
            /// <summary>
            /// 文件夹id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 文件夹名称
            /// </summary>
            public string name { get; set; } = null!;

        }

        public class FolderList
        {
            public List<Folder> data { get; set; } = new List<Folder>();

        }

        /// <summary>
        /// 获取文件夹的下级文件夹信息
        /// </summary>
        public class NextFolder
        {
            /// <summary>
            /// 父级文件夹名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 父级文件夹id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 文件夹信息列表
            /// </summary>
            public Subfolders subfolders { get; set; } = new Subfolders();

        }
        /// <summary>
        /// 
        /// </summary>
        public class Subfolders : MetaPagedDto<Folder>
        {
        }


        /// <summary>
        /// 文件夹下的素材信息
        /// </summary>
        public class FileInfo
        {
            /// <summary>
            /// id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 素材名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 
            /// </summary>
            public int duration { get; set; }

            /// <summary>
            /// 宽度
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// 高度
            /// </summary>
            public int height { get; set; }

            /// <summary>
            /// 缩略图信息
            /// </summary>
            public string? thumbnail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string? type { get; set; }

            /// <summary>
            /// 视频ID
            /// </summary>
            public string? video_id { get; set; }

            /// <summary>
            /// 可以在其中检索图像的临时 URL。不要在广告创作中使用此 URL。
            /// </summary>
            public string? url { get; set; }

            /// <summary>
            /// hash
            /// </summary>
            public string? hash { get; set; }
        }

        /// <summary>
        /// BM素材图片详情
        /// </summary>
        public class BmImagesInfo
        {
            /// <summary>
            /// 可以在其中检索图像的临时 URL。不要在广告创作中使用此 URL。
            /// </summary>

            public string? url { get; set; }
            /// <summary>
            /// 指向调整大小以适应 128x128 像素框的图像版本的临时 URL
            /// </summary>
            public string? url_128 { get; set; }

            /// <summary>
            /// 图片编号id
            /// </summary>

            public string id { get; set; } = null!;

            /// <summary>
            /// 图片素材名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 唯一标识图像的哈希。
            /// </summary>
            public string? hash { get; set; }

        }

    }
}
