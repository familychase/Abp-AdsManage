using Ads.Automation.Application.Contracts.Entity.Publishing.Template;
using Ads.Automation.Domain.Publishing;
using Ads.Automation.Domain.Publishing.BusinessModel.MediaAudience;
using Ads.Automation.Domain.Publishing.BusinessModel.Meta;

namespace Ads.Automation.Application.Mapper;

/// <summary>
/// 发布模板模块的 AutoMapper 映射配置
/// 负责 ViewModel（Application.Contracts 层）与 Domain BO 之间的双向映射
/// </summary>
public class PublishingTemplateAutoMapperProfile : Profile
{
    public PublishingTemplateAutoMapperProfile()
    {
        // ========================================
        // MetaCampaignViewModel → MetaCampaignBo
        // ========================================
        CreateMap<MetaCampaignViewModel, MetaCampaignBo>()
            .ForMember(dest => dest.AdStructSplit, opt => opt.MapFrom(src => src.AdStructSplit ?? string.Empty));

        // ========================================
        // MetaAdsetViewModel → MetaAdsetBo
        // ========================================
        CreateMap<MetaAdsetViewModel, MetaAdsetBo>()
            .ForMember(dest => dest.CustomEventType, opt => opt.MapFrom(src => src.CustomEventType ?? string.Empty))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime ?? string.Empty))
            .ForMember(dest => dest.AdsAttributionSpec, opt => opt.Ignore())
            .ForMember(dest => dest.AttributionSpec, opt => opt.Ignore());

        // ========================================
        // MetaAdViewModel → MetaAdBo
        // ========================================
        CreateMap<MetaAdViewModel, MetaAdBo>()
            .ForMember(dest => dest.AppDeepLink, opt => opt.MapFrom(src => src.AppDeepLink ?? string.Empty))
            .ForMember(dest => dest.WebUrl, opt => opt.MapFrom(src => src.WebUrl ?? string.Empty))
            .ForMember(dest => dest.DisplayUrl, opt => opt.MapFrom(src => src.DisplayUrl ?? string.Empty))
            .ForMember(dest => dest.TrackPixelNo, opt => opt.MapFrom(src => src.TrackPixelNo ?? string.Empty))
            .ForMember(dest => dest.TrackPixelUrl, opt => opt.MapFrom(src => src.TrackPixelUrl ?? string.Empty));

        // ========================================
        // MetaAudienceViewModel → MetaAudienceBo
        // ========================================
        CreateMap<MetaAudienceViewModel, MetaAudienceBo>()
            .ForMember(dest => dest.IsManualPosition, opt => opt.MapFrom(src => src.IsManualPosition ?? false))
            .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType ?? string.Empty))
            .ForMember(dest => dest.AdRmt, opt => opt.Ignore());

        // ========================================
        // AdvancedMaterialTypeViewModel → AdvancedMaterialTypeBo
        // ========================================
        CreateMap<AdvancedMaterialTypeViewModel, AdvancedMaterialTypeBo>()
            .AfterMap((src, dest) =>
            {
                // 根据子属性自动推导标准美化总开关
                dest.SetStandardEnhancements(null!);
            });

        // ========================================
        // MetaPublishAccountDataViewModel → MetaPublishAccountData
        // ========================================
        CreateMap<MetaPublishAccountDataViewModel, MetaPublishAccountData>();

        // ========================================
        // MetaPublishDataViewModel → MetaPublishDataBo
        // ========================================
        CreateMap<MetaPublishDataViewModel, MetaPublishDataBo>()
            .ForMember(dest => dest.CampaignData, opt => opt.MapFrom(src => src.CampaignData))
            .ForMember(dest => dest.AdsetData, opt => opt.MapFrom(src => src.AdsetData))
            .ForMember(dest => dest.AdData, opt => opt.MapFrom(src => src.AdData))
            .ForMember(dest => dest.AudienceData, opt => opt.MapFrom(src => src.AudienceData))
            .ForMember(dest => dest.AccountData, opt => opt.MapFrom(src => src.AccountData));

        // ========================================
        // AdsPublishTemplate → AdsPublishTemplateListDto
        // ========================================
        CreateMap<AdsPublishTemplate, AdsPublishTemplateListDto>()
            .ForMember(dest => dest.Version,
                opt => opt.MapFrom(src => $"V{src.Version + 1}"))
            .ForMember(dest => dest.PublishAdCount,
                opt => opt.MapFrom(src => src.Statistics.PublishAdCount))
            .ForMember(dest => dest.LastPublishTime,
                opt => opt.MapFrom(src => src.LastPublishTime));
    }
}
