/*
 *@author: huangk 2022-12-8 10:18:52
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{
    public partial class MetaOutput
    {

        public class AdImageCopyResult
        {
            public Dictionary<string, ImageInfo> images { get; set; } = new Dictionary<string, ImageInfo>();

            public class ImageInfo
            {
                public string hash { get; set; } = null!;
                public string url { get; set; } = null!;
            }


        }
    }
}
