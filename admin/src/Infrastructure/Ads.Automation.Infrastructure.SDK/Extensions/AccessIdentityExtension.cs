
using RestSharp;

namespace Ads.Automation.Infrastructure.SDK.Extensions;

public static class AccessIdentityExtension
{
    public static RestRequest BearerAuthorizationRequest(this AccessIdentity identity, string segmentUrl)
    {
        return new RestRequest(segmentUrl).AddHeader("Authorization", $"Bearer {identity.AccessToken}");
    }
}
