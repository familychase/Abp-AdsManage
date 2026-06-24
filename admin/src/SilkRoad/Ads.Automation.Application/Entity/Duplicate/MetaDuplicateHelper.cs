using System.Text.RegularExpressions;
using Ads.Automation.Domain.Shared;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Creative;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Input;

namespace Ads.Automation.Application.Entity.Duplicate;

/// <summary>
/// Meta 广告复制公共工具类（纯函数，无 DI 依赖）
/// </summary>
public static class MetaDuplicateHelper
{
    /// <summary>
    /// Campaign API 查询字段
    /// </summary>
    public const string CampaignFields =
        "id,name,status,objective,bid_strategy,daily_budget,lifetime_budget,special_ad_categories,special_ad_category,buying_type,smart_promotion_type,is_skadnetwork_attribution,start_time,stop_time";

    /// <summary>
    /// AdSet API 查询字段
    /// </summary>
    public const string AdSetFields =
        @"id,campaign_id,name,status,daily_budget,lifetime_budget,bid_amount,billing_event,optimization_goal,targeting,start_time,end_time,pacing_type,promoted_object,attribution_spec,destination_type,is_dynamic_creative,daily_spend_cap,lifetime_spend_cap,daily_min_spend_target,lifetime_min_spend_target";

    /// <summary>
    /// Ad API 查询字段
    /// </summary>
    public const string AdFields =
        "id,campaign_id,adset_id,name,status,creative{object_story_id,object_story_spec,asset_feed_spec{images,bodies,titles,descriptions,link_urls,videos,ad_formats,call_to_action_types,asset_customization_rules{customization_spec,body_label,title_label,description_label,link_url_label,image_label,is_default},optimization_type},degrees_of_freedom_spec},creative_asset_groups_spec,ad_format,image_ids,video_id,conversion_domain,conversion_id";

    /// <summary>
    /// 同一账户相邻两次复制的最小间隔（分钟）
    /// </summary>
    public const int DuplicateIntervalMinutes = 30;

    // ============ 日志方法 ============

    /// <summary>
    /// 记录复制错误信息到日志实体
    /// </summary>
    public static void RecordError(AdsDuplicateLogging log, string errorMsg, string operation = "")
    {
        var fullError = errorMsg;
        if (!string.IsNullOrEmpty(operation) && !fullError.StartsWith("["))
            fullError = $"[{operation}] {errorMsg}";

        log.SetErrorMessage(fullError);
        log.SetState(DuplicateState.FAILED);
    }

    // ============ 创意参数构建 ============

    /// <summary>
    /// 从 MetaCreativeDto 构建 AdCreativeAddParameter（用于两步创建流程的先创创意）ao
    /// 提取 name、object_story_spec、asset_feed_spec 等关键字段
    /// </summary>
    public static MetaInput.AdCreativeAddParameter BuildCreativeAddParameter(MetaCreativeDto creative, string name)
    {
        // creative.asset_feed_spec?.asset_customization_rules = null;
        creative.object_story_spec?.instagram_user_id = null;
        creative.use_page_actor_override = true; // 强制使用主页身份发布，避免跨账户复制时因 IG 绑定问题导致创建失败
        if(creative.asset_feed_spec!=null)
        {
            if (!creative.asset_feed_spec.images.IsNullOrEmpty())
            {
                creative.asset_feed_spec.ad_formats = ["SINGLE_IMAGE"];
            }
            
            if(!creative.asset_feed_spec.videos.IsNullOrEmpty())
            {
                creative.asset_feed_spec.ad_formats = ["SINGLE_VIDEO"];
            }
            
            creative.asset_feed_spec.optimization_type = null;
            // if (!creative.asset_feed_spec.link_urls.IsNullOrEmpty())
            // {
            //     creative.asset_feed_spec.link_urls = creative.asset_feed_spec.link_urls.Select(s =>
            //         new MetaAdAssetFeedSpecDto.MetaAdAssetFeedSpecLinkUrlDto()
            //         {
            //             website_url = s.website_url,
            //             deeplink_url = s.deeplink_url
            //         }).DistinctBy(l => new { l.website_url, l.deeplink_url }).ToList();
            // }

            // if (!creative.asset_feed_spec.titles.IsNullOrEmpty())
            // {
            //     creative.asset_feed_spec.titles = creative.asset_feed_spec.titles!
            //         .Where(c => !c.text.IsNullOrEmpty()).ToList();
            // }
            // if (!creative.asset_feed_spec.bodies.IsNullOrEmpty())
            // {
            //     creative.asset_feed_spec.bodies = creative.asset_feed_spec.bodies!
            //         .Where(c => !c.text.IsNullOrEmpty()).ToList();
            // }
            // if (!creative.asset_feed_spec.descriptions.IsNullOrEmpty())
            // {
            //     creative.asset_feed_spec.descriptions = creative.asset_feed_spec.descriptions!
            //         .Where(c => !c.text.IsNullOrEmpty()).ToList();
            //
            //     creative.asset_feed_spec.descriptions = creative.asset_feed_spec.descriptions.IsNullOrEmpty()
            //         ? null
            //         : creative.asset_feed_spec.descriptions;
            //
            //     if (!creative.asset_feed_spec.asset_customization_rules.IsNullOrEmpty())
            //     {
            //         foreach (var rule in creative.asset_feed_spec.asset_customization_rules)
            //         {
            //             rule.description_label = null;
            //         }
            //     }
            // }
        }
        
        return new MetaInput.AdCreativeAddParameter
        {
            name = name,
            object_story_spec = creative.object_story_spec ?? new MetaAdCreativeObjectStorySpecDto(),
            asset_feed_spec = creative.asset_feed_spec,
            degrees_of_freedom_spec = null,
            authorization_category = creative.authorization_category,
            use_page_actor_override = true
        };
    }
    
      /// <summary>
    /// 从 MetaCreativeDto 构建 AdCreativeAddParameter（用于两步创建流程的先创创意）ao
    /// 提取 name、object_story_spec、asset_feed_spec 等关键字段
    /// </summary>
    public static MetaCreativeDto BuildAdCreativeParameter(MetaCreativeDto creative, string name)
    {
        // creative.asset_feed_spec?.asset_customization_rules = null;
        
        if(creative.asset_feed_spec!=null)
        {
            creative.asset_feed_spec.ad_formats = ["SINGLE_IMAGE"];
            creative.asset_feed_spec.optimization_type = null;
            // if (!creative.asset_feed_spec.link_urls.IsNullOrEmpty())
            // {
            //     creative.asset_feed_spec.link_urls = creative.asset_feed_spec.link_urls.Select(s =>
            //         new MetaAdAssetFeedSpecDto.MetaAdAssetFeedSpecLinkUrlDto()
            //         {
            //             website_url = s.website_url,
            //             deeplink_url = s.deeplink_url
            //         }).DistinctBy(l => new { l.website_url, l.deeplink_url }).ToList();
            // }
            // if(!creative.asset_feed_spec.images.IsNullOrEmpty())
            // {
            //     creative.asset_feed_spec.images!.ForEach(e=>e.adlabels=null);
            //     
            //     creative.asset_feed_spec.images = creative.asset_feed_spec.images
            //         .DistinctBy(i=>i.hash).ToList();
            // }
            //
            // if(!creative.asset_feed_spec.videos.IsNullOrEmpty())
            // { 
            //     creative.asset_feed_spec.videos!.ForEach(e=>e.adlabels=null);
            //     
            //     creative.asset_feed_spec.videos = creative.asset_feed_spec.videos
            //         .DistinctBy(i=>new { i.video_id,i.thumbnail_url }).ToList();
            // }

            if (!creative.asset_feed_spec.titles.IsNullOrEmpty())
            {
                creative.asset_feed_spec.titles = creative.asset_feed_spec.titles!
                    .Where(c => !c.text.IsNullOrEmpty()).ToList();
            }
            if (!creative.asset_feed_spec.bodies.IsNullOrEmpty())
            {
                creative.asset_feed_spec.bodies = creative.asset_feed_spec.bodies!
                    .Where(c => !c.text.IsNullOrEmpty()).ToList();
            }
            if (!creative.asset_feed_spec.descriptions.IsNullOrEmpty())
            {
                creative.asset_feed_spec.descriptions = creative.asset_feed_spec.descriptions!
                    .Where(c => !c.text.IsNullOrEmpty()).ToList();
            }
        }
        
        return new MetaCreativeDto
        {
            object_story_spec = creative.object_story_spec ?? new MetaAdCreativeObjectStorySpecDto(),
            asset_feed_spec = creative.asset_feed_spec,
            degrees_of_freedom_spec = null,
            authorization_category = creative.authorization_category,
            use_page_actor_override = true
        };
    }

    // ============ 素材构建 ============

    /// <summary>
    /// 一次性构建广告 creative 素材结构
    /// 深拷贝源广告后执行 BuildAd，后续循环只需重置标识字段，素材结构直接复用
    /// </summary>
    public static List<Ad> BuildAdsOnce(List<Ad> sourceAds,
        AdCampaign? builtCampaign, AdSet? builtAdset, int index,
        string smartPromotionType, bool isInternal, string? targetPageId)
    {
        
        return sourceAds.Select(src =>
        {
            var json = JsonSerializer.Serialize(src, JsonHelper.JsonSerializerOptions);
            var clone = JsonSerializer.Deserialize<Ad>(json,  JsonHelper.JsonSerializerOptions);

            BuildAd(clone, builtAdset, index, smartPromotionType, isInternal, targetPageId);
            return clone;
        }).ToList();
    }

    // ============ 复制命名 ============

    /// <summary>
    /// 构建复制广告名称后缀（英文格式）
    /// 自动剥离已有的复制后缀（兼容中文「复制」和英文「copy」两种旧格式），
    /// 追加统一英文格式 " (copy_{index})"
    /// </summary>
    public static string BuildCopyName(string originalName, int index)
    {
        // 剥离旧后缀：中文括号/英文括号 + 复制/copy + 下划线 + 数字
        var cleaned = Regex.Replace(originalName, @"[（(](?:复制|copy)_\d+[）)]$", "",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        return $"{cleaned} (copy_{index})";
    }

    // ============ 参数构建 ============

    /// <summary>
    /// 构建复制用的 Campaign 参数
    /// 清除 id/account_id，添加复制序号后缀
    /// 注意：此方法会原地修改 source 对象
    /// </summary>
    public static AdCampaign BuildCampaign(AdCampaign source, int index)
    {
        source.id = null!;
        source.account_id = null!;
        source.special_ad_categories = new List<string>();
        source.name = BuildCopyName(source.name, index);
        source.is_skadnetwork_attribution = source.is_skadnetwork_attribution == true ? true : (bool?)null;
        return source;
    }

    /// <summary>
    /// 构建复制用的 AdSet 参数
    /// 添加复制序号后缀，修正开始时间
    /// 注意：此方法会原地修改 source 对象
    /// </summary>
    public static AdSet BuildAdSet(AdSet source, int index, string smartPromotionType)
    {
        source.name = BuildCopyName(source.name, index);
        if (source.start_time.HasValue && source.start_time < DateTime.UtcNow)
            source.start_time = DateTime.UtcNow;
        source.SetParameterIsNull(smartPromotionType);
        return source;
    }

    /// <summary>
    /// 构建复制用的 Ad 参数
    /// 清除 creative id、重置标识字段、处理 video image_hash、
    /// 内部复制清除子素材引用，外部复制替换 page_id
    /// 注意：此方法会原地修改 ad 对象
    /// </summary>
    public static void BuildAd(Ad ad, AdSet? adset, int index,
        string smartPromotionType, bool isInternal, string? targetPageId = null)
    {
        ad.id = null!;
        ad.name = BuildCopyName(ad.name, index);
        ad.status = "ACTIVE";

        if (ad.creative != null)
        {
            ad.creative.id = null!;
            ad.creative.authorization_category = smartPromotionType == "SMART_APP_PROMOTION" ? null! : "NONE";

            if (isInternal)
            {
                var linkData = ad.creative.object_story_spec?.link_data;
                if (linkData?.child_attachments?.Count > 0)
                    foreach (var child in linkData.child_attachments)
                    {
                        child.picture = null!;  // 只用 image_hash，不走外链
                        if (!string.IsNullOrEmpty(child.video_id)) child.image_hash = null!;
                    }
            }

            if (ad.creative.object_story_spec?.video_data != null)
                ad.creative.object_story_spec.video_data.image_hash = null!;

            var imgAbleName = new List<string>(); 
            // // 清除 asset_feed_spec 中所有图片的 image_crops（创建广告时不需要裁切信息）
            if (ad.creative.asset_feed_spec?.images?.Count > 0)
            {
                // 如果img所有的hash一致 则直接引用第一个 不一致则更改为最新的
                
                var hash = ad.creative.asset_feed_spec.images.Select(s=>s.hash).Distinct().ToList();
                if (hash.Count == 1)
                {
                    ad.creative.asset_feed_spec.images = new List<MetaAdAssetFeedSpecDto.MetaAdAssetFeedSpecImageDto>()
                    {
                        new MetaAdAssetFeedSpecDto.MetaAdAssetFeedSpecImageDto
                        {
                            hash = hash[0]
                        }
                    };
                }
            }

            if (ad.creative_asset_groups_spec?.groups?.Count > 0)
                foreach (var group in ad.creative_asset_groups_spec.groups)
                    if (group.videos?.Count > 0)
                        foreach (var video in group.videos) video.image_hash = null!;

            if (!string.IsNullOrEmpty(targetPageId))
                ReplacePageId(ad.creative, targetPageId);

            CopyMultilingualAdData(ad.creative,imgAbleName);
        }
    }

    /// <summary>
    /// 替换 creative 中 object_story_spec 的 page_id 为目标主页 ID
    /// </summary>
    public static void ReplacePageId(MetaCreativeDto creative, string targetPageId)
    {
        if (creative.object_story_spec != null)
            creative.object_story_spec.page_id = targetPageId;
    }

    // ============ 多语言复制 ============

    /// <summary>
    /// 复制多语言广告数据
    /// 包括 asset_feed_spec 中的 adlabels、asset_customization_rules 和 dynamic_ad_creative_data
    /// adlabels 只保留 name（跨账户时 Meta 自动生成 id）
    /// asset_customization_rules 保留完整的语言-素材映射规则
    /// </summary>
    public static void CopyMultilingualAdData(MetaCreativeDto creative,List<string> imgAbleName)
    {
        var feedSpec = creative.asset_feed_spec;
        if (feedSpec != null)
        {
            // 创意级 adlabels
            if (feedSpec.adlabels?.Count > 0)
                feedSpec.adlabels = feedSpec.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            // 图片级 adlabels
            if (feedSpec.images?.Count > 0)
                foreach (var img in feedSpec.images)
                    if (img.adlabels?.Count > 0)
                        img.adlabels = img.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            // 标题级 adlabels
            if (feedSpec.titles?.Count > 0)
                foreach (var title in feedSpec.titles)
                    if (title.adlabels?.Count > 0)
                        title.adlabels = title.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            // 正文级 adlabels
            if (feedSpec.bodies?.Count > 0)
                foreach (var body in feedSpec.bodies)
                    if (body.adlabels?.Count > 0)
                        body.adlabels = body.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            // 描述级 adlabels
            if (feedSpec.descriptions?.Count > 0)
                foreach (var desc in feedSpec.descriptions)
                    if (desc.adlabels?.Count > 0)
                        desc.adlabels = desc.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            // 链接级 adlabels
            if (feedSpec.link_urls?.Count > 0)
                foreach (var url in feedSpec.link_urls)
                    if (url.adlabels?.Count > 0)
                        url.adlabels = url.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            // 视频级 adlabels
            if (feedSpec.videos?.Count > 0)
                foreach (var video in feedSpec.videos)
                    if (video.adlabels?.Count > 0)
                        video.adlabels = video.adlabels.Select(l => new AdLabelDto { name = l.name }).ToList();

            
            // 多语言核心：asset_customization_rules
            // 保留完整的语言-素材映射规则（locales + label references）
            // label references 只使用 name，不依赖 ID，可直接保留
            if (feedSpec.asset_customization_rules?.Count > 0)
            {
                // if (feedSpec.asset_customization_rules.Any(c => c.image_label != null))
                // {
                //     feedSpec.asset_customization_rules = feedSpec.asset_customization_rules
                //         .Where(c => imgAbleName.Contains(c.image_label.name)).ToList();
                // }

                feedSpec.asset_customization_rules = feedSpec.asset_customization_rules
                    .Select(r => new AssetCustomizationRuleDto
                    {
                        customization_spec = r.customization_spec != null
                            ? r.customization_spec
                            : null,
                        body_label = r.body_label?.name != null
                            ? new AssetLabelReferenceDto { name = r.body_label.name }
                            : null,
                        title_label = r.title_label?.name != null
                            ? new AssetLabelReferenceDto { name = r.title_label.name }
                            : null,
                        description_label = r.description_label?.name != null
                            ? new AssetLabelReferenceDto { name = r.description_label.name }
                            : null,
                        link_url_label = r.link_url_label?.name != null
                            ? new AssetLabelReferenceDto { name = r.link_url_label.name }
                            : null,
                        image_label = r.image_label?.name != null
                            ? new AssetLabelReferenceDto { name = r.image_label.name }
                            : null,
                        video_label = r.video_label?.name != null
                            ? new AssetLabelReferenceDto { name = r.video_label.name }
                            : null,
                        is_default = r.is_default,
                    }).ToList();
                
                // 对数据内容去重复
                
                // 检查是否存在 默认 没有默认设置第一个为默认
                if (!feedSpec.asset_customization_rules.Any(r => r.is_default.HasValue && r.is_default.Value))
                {
                    feedSpec.asset_customization_rules[0].is_default = true;
                }
            }
        }

        // 按国家分发的多语言文案（dynamic_ad_creative_data）
        // var spec = creative.object_story_spec;
        // if (spec?.dynamic_ad_creative_data?.Count > 0)
        // {
        //     spec.dynamic_ad_creative_data = spec.dynamic_ad_creative_data
        //         .Select(x => new DynamicAdCreativeDataItemDto
        //         {
        //             country = x.country,
        //             body = x.body,
        //             title = x.title,
        //             link_description = x.link_description,
        //             call_to_action_text = x.call_to_action_text,
        //             deeplink_url = x.deeplink_url
        //         }).ToList();
        // }
    }

    // ============ 素材提取/替换（跨账户使用） ============

    /// <summary>
    /// 从广告列表中提取所有图片 hash 和视频 ID
    /// 遍历 creative、creative_asset_groups_spec、asset_feed_spec 等所有素材来源
    /// </summary>
    public static (HashSet<string> images, HashSet<string> videos) ExtractMaterials(List<Ad> ads)
    {
        var imageHashSet = new HashSet<string>();
        var videoIdSet = new HashSet<string>();

        foreach (var ad in ads)
        {
            var creative = ad.creative;
            if (creative == null) continue;

            if (ad.creative_asset_groups_spec?.groups?.Count > 0)
                foreach (var group in ad.creative_asset_groups_spec.groups)
                {
                    if (group.images?.Count > 0) foreach (var img in group.images) imageHashSet.Add(img.hash);
                    if (group.videos?.Count > 0) foreach (var vid in group.videos) videoIdSet.Add(vid.video_id);
                }

            var linkData = creative.object_story_spec?.link_data;
            if (linkData != null)
            {
                if (linkData.child_attachments?.Count > 0)
                    foreach (var child in linkData.child_attachments)
                        if (!string.IsNullOrEmpty(child.image_hash)) imageHashSet.Add(child.image_hash);
                else if (!string.IsNullOrEmpty(linkData.image_hash))
                    imageHashSet.Add(linkData.image_hash);
            }

            var videoData = creative.object_story_spec?.video_data;
            if (videoData != null && !string.IsNullOrEmpty(videoData.video_id))
                videoIdSet.Add(videoData.video_id);

            var dynamicSpec = creative.asset_feed_spec;
            if (dynamicSpec != null)
            {
                if (dynamicSpec.images?.Count > 0) foreach (var img in dynamicSpec.images) imageHashSet.Add(img.hash);
                if (dynamicSpec.videos?.Count > 0) foreach (var vid in dynamicSpec.videos) videoIdSet.Add(vid.video_id);
            }
        }

        return (imageHashSet, videoIdSet);
    }

    /// <summary>
    /// 替换广告中的素材引用为跨账户拷贝后的新 hash 和新视频 ID
    /// </summary>
    public static void ReplaceAdMaterials(Ad ad,
        Dictionary<string, string> imageDic, Dictionary<string, (string videoNo, string coverUrl)> videoDic)
    {
        var creative = ad.creative;
        if (creative == null) return;

        if (ad.creative_asset_groups_spec?.groups?.Count > 0)
            foreach (var group in ad.creative_asset_groups_spec.groups)
            {
                if (group.images?.Count > 0)
                    foreach (var img in group.images)
                        if (imageDic.TryGetValue(img.hash, out var newHash)) img.hash = newHash;
                if (group.videos?.Count > 0)
                    foreach (var vid in group.videos)
                        if (videoDic.TryGetValue(vid.video_id, out var newVid))
                        { vid.video_id = newVid.videoNo; vid.image_hash = string.Empty; }
            }

        var linkData = creative.object_story_spec?.link_data;
        if (linkData != null)
        {
            if (linkData.child_attachments?.Count > 0)
                foreach (var child in linkData.child_attachments)
                    if (imageDic.TryGetValue(child.image_hash, out var newHash)) child.image_hash = newHash;
            else if (!string.IsNullOrEmpty(linkData.image_hash) && imageDic.TryGetValue(linkData.image_hash, out var h))
                linkData.image_hash = h;
        }

        var videoData = creative.object_story_spec?.video_data;
        if (videoData != null && !string.IsNullOrEmpty(videoData.video_id))
            if (videoDic.TryGetValue(videoData.video_id, out var newVid))
            { videoData.video_id = newVid.videoNo; videoData.image_url = newVid.coverUrl; }

        var dynamicSpec = creative.asset_feed_spec;
        if (dynamicSpec != null)
        {
            if (dynamicSpec.images?.Count > 0)
                foreach (var img in dynamicSpec.images)
                    if (imageDic.TryGetValue(img.hash, out var newHash)) img.hash = newHash;
            if (dynamicSpec.videos?.Count > 0)
                foreach (var vid in dynamicSpec.videos)
                    if (videoDic.TryGetValue(vid.video_id, out var newVid))
                    { vid.video_id = newVid.videoNo; vid.thumbnail_url = newVid.coverUrl; }
        }
    }
}
