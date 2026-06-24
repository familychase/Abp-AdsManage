using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Threading;

namespace Ads.Automation.EntityFrameworkCore
{
    public class AdsAutomationEfCoreEntityExtensionMappings
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

        public static void Configure()
        {
            OneTimeRunner.Run(() =>
            {
                // Configure extra properties for the entities defined in the modules used by your application.
                // This class can be used
            });
        }
    }
}
