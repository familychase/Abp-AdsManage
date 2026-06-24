
namespace Ads.Automation.Application
{
    public class AdsAutomationApplicationAutoMapperProfile : Profile
    {
        /// <summary>
        /// 自动映射关系配置
        /// </summary>
        public AdsAutomationApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
            * Alternatively, you can split your mapping configurations
            * into multiple profile classes for a better organization. */

            CreateMap<SysUser, SysUserDto>()
                .ForMember(e => e.UserId, opt => opt.MapFrom(s => s.Id))
                .ForMember(e => e.LastLoginTime, opt => opt.MapFrom(s => s.LastLoginTime.HasValue ? s.LastLoginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""))
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<SysUser, UserInfoDto>()
                .ForMember(e => e.UserId, opt => opt.MapFrom(s => s.Id))
                .ForMember(e => e.LastLoginTime, opt => opt.MapFrom(s => s.LastLoginTime.HasValue ? s.LastLoginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""))
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<SysRoles, SysRolesDto>()
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<SysMenus, SysMenusDto>();
            CreateMap<SysDepartment, SysDepartmentDto>()
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<AdsChannel, AdsChannelDto>();
            CreateMap<AdsChannel, AdsChannelListDto>();
            CreateMap<AdsAccount, AdsAccountDto>();
            CreateMap<AdsSyncSchedule, AdsSyncScheduleDto>();
            CreateMap<AdsDuplicateDetail, AdsDuplicateDetailDto>();
            CreateMap<AdsPage, PagesDto>()
                .ForMember(e => e.LastSyncTime, opt => opt.MapFrom(s => s.LastSyncTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<AdsPixel, PixelDto>()
                .ForMember(e => e.LastSyncTime, opt => opt.MapFrom(s => s.LastSyncTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(e => e.AccountCount, opt => opt.Ignore())
                .ForMember(e => e.AssociatedAccounts, opt => opt.Ignore());
            CreateMap<SysDictSort, SysDictSortDto>()
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<SysDictItem, SysDictItemDto>();

            CreateMap<SysDictItem, SysDictItemTreeNodeDto>()
                .IncludeBase<SysDictItem, SysDictItemDto>();

            CreateMap<SysDictItem, SysDictItemDetailDto>()
                .IncludeBase<SysDictItem, SysDictItemDto>()
                .ForMember(e => e.CreationTime, opt => opt.MapFrom(s => s.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}
