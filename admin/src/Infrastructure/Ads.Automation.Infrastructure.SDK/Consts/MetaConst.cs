using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Infrastructure.SDK.Consts
{
    public class MetaConst
    {
        public const string Fields = "fields";

        public const string Limit = "limit";

        public const string Summary = "summary";

        //还包括为支持投放广告而创建的 INLINE_CREATE 帖子。
        public const string IncludeInlineCreate = "include_inline_create";


        public const string AccountFields = "id,account_id,name,account_status,spend_cap,amount_spent,currency,timezone_name,timezone_offset_hours_utc,business,created_time,agencies{id,name},disable_reason,age";

        public const string PageFields = "id,name,category";

        public const string CampaignFields =
            "account_id,id,name,status,daily_budget,lifetime_budget,objective,updated_time,smart_promotion_type,created_time";

        public const string AdSetFields =
            "id,account_id,campaign_id,name,status,daily_budget,lifetime_budget,optimization_goal,promoted_object,bid_strategy,billing_event,targeting,attribution_spec,updated_time,campaign{bid_strategy},created_time,is_dynamic_creative,learning_stage_info,destination_type,is_incremental_attribution_enabled,start_time,end_time";

        public const string AdFields = "id,account_id,campaign_id,adset_id,name,status,updated_time,creative_asset_groups_spec,creative{id,asset_feed_spec,object_story_spec,object_story_id,contextual_multi_ads},adset{is_dynamic_creative},created_time";

        public const string AdActivityFields = "actor_id,actor_name,application_id,application_name,date_time_in_timezone,event_time,event_type,object_id,extra_data,object_name,object_type,translated_event_type";

        /// <summary>
        /// 广告系列、广告组和广告发布的详细字段
        /// </summary>
        public const string CampaignPublishFields = "id,name,objective,buying_type,daily_budget,lifetime_budget,bid_strategy,pacing_type,promoted_object,smart_promotion_type,is_skadnetwork_attribution,status,spend_cap,special_ad_categories,special_ad_category_country";

        public const string AdSetPublishDetailFields = "id,name,campaign_id,optimization_goal,billing_event,promoted_object,daily_budget,lifetime_budget,bid_strategy,pacing_type,bid_amount,daily_min_spend_target,daily_spend_cap,lifetime_min_spend_target,lifetime_spend_cap,attribution_spec,targeting,is_dynamic_creative,dsa_beneficiary,dsa_payor,start_time,end_time,bid_constraints,regional_regulation_identities,regional_regulated_categories,is_incremental_attribution_enabled";

        public const string AdPublishDetailFields = "id,name,adset_id,creative{object_story_id,asset_feed_spec,object_story_spec,authorization_category,use_page_actor_override,contextual_multi_ads},effective_status,creative_asset_groups_spec";


        public const string AdEditDetailFields = "id,account_id,campaign_id,adset_id,name,status,creative{id,asset_feed_spec,object_story_spec,degrees_of_freedom_spec}";

        #region  扩展属性

        public const string interestType = "adinterest";
        public const string countryType = "adgeolocation";

        #endregion

        #region  用户基本信息

        public const string userFields = "id,name,email,businesses{id,name}";

        #endregion

        #region BM素材
        public const string FolderFields = "id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name}}}}}}}}}}}}}}}";

        // public const string BMFolderFields = "businesses{id,name,creative_folders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name,subfolders.limit(1000){id,name}}}}}}}}}}}}}}}}}}";

        //BM只拉第一层文件夹
        public const string BMFolderFields = "businesses{id,name,creative_folders.limit(1000){id,name}}";

        public const string BmFolderMaterial = "id,name,thumbnail,type,url,video_id,hash";

        //BM子文件夹
        public const string BmSubFolderFields = "id,name";

        #endregion

        //像素字段
        public const string PixelFileds = "id,name,stats";

        //受众字段
        public const string AudienceFileds = "id,subtype,lookalike_spec,rule,name,approximate_count_lower_bound,approximate_count_upper_bound,delivery_status";

        //应用字段
        public const string ApplicationFileds = "id,icon_url,name,object_store_urls,supported_platforms";

        #region 广告编辑字段

        public const string AdEditCampaignFields =
            $"id,name,status,daily_budget,lifetime_budget,objective,bid_strategy,updated_time,smart_promotion_type,promoted_object";

        public const string AdEditAdSetFields =
            $"id,name,start_time,end_time,status,daily_budget,lifetime_budget,optimization_goal,promoted_object,bid_amount,bid_strategy,billing_event,targeting,attribution_spec,is_dynamic_creative,pacing_type,daily_min_spend_target,daily_spend_cap,lifetime_min_spend_target,lifetime_spend_cap,updated_time,bid_constraints,dsa_beneficiary,dsa_payor,regional_regulation_identities,regional_regulated_categories,is_incremental_attribution_enabled";

        public const string AdEditAdFields =
            $"id,name,status,effective_status,updated_time,creative_asset_groups_spec,conversion_domain,tracking_specs,creative{{id,asset_feed_spec,object_story_id,object_story_spec,degrees_of_freedom_spec,authorization_category,use_page_actor_override,contextual_multi_ads}},adset{{is_dynamic_creative}}";


        public const string AdEditAdSetUpdateFields =
            $"id,name,end_time,status,daily_budget,lifetime_budget,promoted_object,bid_amount,targeting,pacing_type,daily_min_spend_target,daily_spend_cap,lifetime_min_spend_target,lifetime_spend_cap,bid_constraints,optimization_goal,dsa_beneficiary,dsa_payor,regional_regulation_identities,regional_regulated_categories";

        public const string AdEditAdUpdateFields = $"id,account_id,campaign_id,adset_id,name,status,creative_asset_groups_spec,creative{{id,asset_feed_spec,object_story_id,object_story_spec,degrees_of_freedom_spec,contextual_multi_ads}},adset{{is_dynamic_creative}},campaign{{smart_promotion_type,objective}}";
        #endregion

        public const string PostsFields = "id,attachments{media_type,title,media},shares,is_published";
    }
}
