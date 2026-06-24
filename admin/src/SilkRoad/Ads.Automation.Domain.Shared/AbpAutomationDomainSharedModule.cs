using System.Reflection;
using System.Text.Json;
using Ads.Automation.Domain.Shared.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Ads.Automation.Domain.Shared
{
    [DependsOn(
        typeof(AbpLocalizationModule),
        typeof(AbpValidationModule)
    )]
    public class AbpAutomationDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpAutomationDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.DefaultResourceType = typeof(AdsAutomationResource);
                options.Resources
                    .Add<AdsAutomationResource>("zh-Hans")
                    .AddVirtualJson("/Localization/AdsAutomation");
            });

            // 直接注册 IStringLocalizer<AdsAutomationResource>，绕过 ABP 的虚拟文件系统 bug
            context.Services.AddSingleton<IStringLocalizer<AdsAutomationResource>>(
                new EmbeddedJsonStringLocalizer<AdsAutomationResource>(
                    typeof(AdsAutomationResource).Assembly,
                    "Ads.Automation.Domain.Shared.Localization.AdsAutomation"));
        }
    }
}

/// <summary>
/// 直接从嵌入资源读取 JSON 翻译，绕过 ABP AddVirtualJson 在 10.4 的 bug。
/// </summary>
public class EmbeddedJsonStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly Dictionary<string, Dictionary<string, string>> _cultures = [];

    public EmbeddedJsonStringLocalizer(Assembly assembly, string baseResourceName)
    {
        foreach (var culture in new[] { "zh-Hans", "en" })
        {
            var dict = new Dictionary<string, string>();
            try
            {
                var path = $"{baseResourceName}.{culture}.json";
                using var stream = assembly.GetManifestResourceStream(path);
                if (stream != null)
                {
                    using var doc = JsonDocument.Parse(stream);
                    foreach (var p in doc.RootElement.EnumerateObject())
                        dict[p.Name] = p.Value.GetString() ?? p.Name;
                }
            }
            catch { }
            _cultures[culture] = dict;
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
            if (_cultures.TryGetValue(culture, out var dict) && dict.TryGetValue(name, out var value))
                return new LocalizedString(name, value, resourceNotFound: false);
            if (_cultures.TryGetValue("zh-Hans", out var zh) && zh.TryGetValue(name, out var zhVal))
                return new LocalizedString(name, zhVal, resourceNotFound: false);
            return new LocalizedString(name, name, resourceNotFound: true, searchedLocation: "embedded");
        }
    }

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(this[name].Value, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        if (_cultures.TryGetValue(culture, out var dict))
            return dict.Select(kv => new LocalizedString(kv.Key, kv.Value));
        return _cultures.GetValueOrDefault("zh-Hans", new())
            .Select(kv => new LocalizedString(kv.Key, kv.Value));
    }
}
