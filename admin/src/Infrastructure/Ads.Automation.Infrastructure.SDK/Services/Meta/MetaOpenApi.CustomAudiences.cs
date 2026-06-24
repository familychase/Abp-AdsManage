
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取账户所有自定义受众信息
        /// https://developers.facebook.com/docs/marketing-api/reference/custom-audience/
        /// </summary>
        /// <param name="accountNo">广告账户ID</param>
        /// <param name="identity"></param>
        /// <param name="limit">页面大小</param>
        /// <param name="fields">默认可返回字段列fields= "id,subtype,lookalike_spec,rule,name,approximate_count_lower_bound,approximate_count_upper_bound,delivery_status"</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.CustomAudience>> GetCustomAudiencesAsync(AccessIdentity identity, string accountNo, int limit, string fields)
        {
            var request = identity.Request($"/{accountNo}/customaudiences")
                .AddQueryParameter(MetaConst.Fields, fields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.CustomAudience>>(request))!;
        }


        /// <summary>
        /// 新增账户自定义受众
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">账户信息</param>
        /// <param name="customAudience">受众参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> CustomAudienceAddAsync(AccessIdentity identity, string accountNo, MetaInput.CreateCustomAudience customAudience)
        {
            var request = identity.Request($"/{accountNo}/customaudiences?").UseVersion(ApiDefaultVersion);

            var document = JsonSerializer.SerializeToDocument(
                customAudience,
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            foreach (JsonProperty prop in document.RootElement.EnumerateObject())
            {
                request.AddParameter(prop.Name, prop.Value.ToString());
            }

            return (await Client(UrlConst.MetaGraphApi).PostAsync<MetaOutput.IdResult>(request))!;
        }


        /// <summary>
        /// 兴趣搜索
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="keyword"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Interest>> GetInterestAsync(AccessIdentity identity, string keyword, int limit)
        {
            var request = identity.Request($"/search")
                .AddQueryParameter("q", keyword)
                .AddQueryParameter("type", MetaConst.interestType)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Interest>>(request))!;
        }


        /// <summary>
        /// 地域搜索
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="keyword"></param>
        /// <param name="locationTypes"></param>
        /// <param name="limit"></param>
        /// <param name="locale">语言</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Country>> GetCountryAsync(AccessIdentity identity, string keyword, string[] locationTypes, int limit,string? locale)
        {
            var request = identity.Request($"/search")
             .AddQueryParameter("q", keyword)
             .AddQueryParameter("type", MetaConst.countryType)
             .AddQueryParameter("location_types", locationTypes.ToJsonIgnoreNullValue())
             .AddQueryParameter("locale",locale)
             .AddQueryParameter(MetaConst.Limit, limit)
             .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Country>>(request))!;
        }


        /// <summary>
        /// 细分定位搜索
        /// v18.0 https://developers.facebook.com/docs/marketing-api/audiences/reference/detailed-targeting/?translation
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="account_no"></param>
        /// <param name="keyword">关键字</param>
        /// <param name="limit">字段限制</param>
        /// <param name="limit_type">受众类型 work_employers、work_positions、education_majors、education_schools</param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.SubdivisionTargetingSearch>> GetSubdivisionTargetingSearchAsync(AccessIdentity identity, string account_no, string keyword, int limit, string limit_type = null!, string? locale = null!)
        {
            var request = identity.Request($"{account_no}/targetingsearch")
                .AddQueryParameter("q", keyword)
                .UseVersion(ApiDefaultVersion);

            if (limit > 0)
            {
                request.AddQueryParameter(MetaConst.Limit, limit);
            }

            if (!string.IsNullOrEmpty(limit_type))
            {
                //类型
                request.AddQueryParameter(nameof(limit_type), limit_type);
            }

            if (!string.IsNullOrEmpty(locale))
            {
                //语言
                request.AddQueryParameter(nameof(locale), locale);
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.SubdivisionTargetingSearch>>(request))!;
        }


        /// <summary>
        /// 细分定位建议
        /// v18.0 https://developers.facebook.com/docs/marketing-api/audiences/reference/detailed-targeting/?translation
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="account_no"></param>
        /// <param name="targeting_list">关键字</param>
        /// <param name="limit">字段限制</param>
        /// <param name="limit_type">受众类型 work_employers、work_positions、education_majors、education_schools</param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.SubdivisionTargetingSearch>> GetSubdivisionTargetingSuggestionsAsync(AccessIdentity identity, string account_no, List<SubdivisionTargetingSuggestions>? targeting_list, int limit, string? locale = null!, List<string> limit_type = null!)
        {
            var request = identity.Request($"{account_no}/targetingsuggestions")
                .UseVersion(ApiDefaultVersion);

            if (targeting_list != null && targeting_list.Any())
            {
                request.AddQueryParameter(nameof(targeting_list), targeting_list.ToJsonIgnoreNullValue());
            }

            if (limit > 0)
            {
                request.AddQueryParameter(MetaConst.Limit, limit);
            }

            //   var subdivisionPositioningEnum = new List<string>() { "interests", "behaviors", "life_events", "family_statuses", "industries", "income" };
            if (limit_type != null && limit_type.Any())
            {
                //类型
                request.AddQueryParameter(nameof(limit_type), limit_type.ToJsonIgnoreNullValue());
            }

            if (!string.IsNullOrEmpty(locale))
            {
                //语言
                request.AddQueryParameter(nameof(locale), locale);
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.SubdivisionTargetingSearch>>(request))!;
        }
    }
}
