using Ads.Automation.Domain.Shared;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Ads.Automation.Domain
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpAutomationDomainSharedModule)
    )]
    public class AdsAutomationDomainModule : AbpModule
    {
    }
}
