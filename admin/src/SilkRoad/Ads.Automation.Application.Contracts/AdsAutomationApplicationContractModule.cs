using Ads.Automation.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Ads.Automation.Application.Contracts
{
    [DependsOn(
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule),
        typeof(AbpAutomationDomainSharedModule)
    )]
    public class AdsAutomationApplicationContractModule : AbpModule
    {

    }
}
