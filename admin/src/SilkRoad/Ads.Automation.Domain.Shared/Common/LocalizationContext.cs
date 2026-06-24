using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;

namespace Ads.Automation.Domain.Shared.Common
{
    public class LocalizationContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="languageType"></param>
        public LocalizationContext(GlobalLanguageType languageType) : this()
        {
            this.LanguageType = languageType;
        }

        public LocalizationContext() { }

        /// <summary>
        /// 语言类型
        /// </summary>
        public GlobalLanguageType LanguageType {  get; set; } 
    }
}
