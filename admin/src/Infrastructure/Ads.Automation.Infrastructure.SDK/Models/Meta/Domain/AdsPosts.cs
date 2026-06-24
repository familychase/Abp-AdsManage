

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// https://developers.facebook.com/docs/graph-api/reference/page/ads_posts
        /// </summary>
        public class AdsPosts
        {
            /// <summary>
            /// 帖子 ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 表示是否发布了预定的帖子（仅适用于预定的页面帖子，对于用户帖子和即时发布的帖子，此值始终为真）
            /// </summary>
            public bool is_published { get; set; }

            /// <summary>
            /// 帖子是否已过期
            /// </summary>
            public bool is_expired { get; set; }

            /// <summary>
            /// 帖子中写的信息
            /// </summary>
            public string? message { get; set; }

            /// <summary>
            /// 操作链接
            /// </summary>
            public List<PostsActions>? actions { get; set; }

            /// <summary>
            /// 行动号召
            /// </summary>
            public PostsCallToAction? call_to_action { get; set; }

            /// <summary>
            /// 与故事相关的任何附件
            /// </summary>
            public MetaPagedDto<MetaDomain.Attachments>? attachments { get; set; }

            /// <summary>
            /// 帖子是否可以使用不同的视频购买选项进行推广。如果视频符合条件，则返回空列表。否则，返回帖子无法推广的原因列表。
            /// </summary>
            public List<string>? video_buying_eligibility { get; set; }

            /// <summary>
            /// 分享数量
            /// </summary>
            public PostsSharedDto? shares { get; set; }

            /// <summary>
            /// 此帖子是否可以在 Instagram 上推广
            /// </summary>
            public bool is_instagram_eligible { get; set; }

            /// <summary>
            /// 评论
            /// </summary>
            public MetaPagedDto<Object> comments { get; set; }

            /// <summary>
            /// 点赞
            /// </summary>
            public MetaPagedDto<Object> likes { get; set; }

        }

        /// <summary>
        /// 分享信息
        /// </summary>
        public class PostsSharedDto
        {
            /// <summary>
            /// 分享数量
            /// </summary>
            public int count { get; set; }
        }

        public class PostsCallToAction
        {
            /// <summary>
            /// 行动号召
            /// </summary>
            public string? type { get; set; }

            /// <summary>
            /// 行动号召信息
            /// </summary>
            public PostsCallToActionLink? value { get; set; }


            public class PostsCallToActionLink
            {
                /// <summary>
                /// link标题
                /// </summary>
                public string? link_title { get; set; }

                /// <summary>
                /// link地址
                /// </summary>
                public string? link { get; set; }

                /// <summary>
                /// link格式
                /// </summary>
                public string? link_format { get; set; }
            }
        }

        public class PostsActions
        {
            /// <summary>
            /// 类型名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 链接地址
            /// </summary>
            public string link { get; set; } = null!;
        }


    }

}
