using EasyCaching.Core;
using RestSharp;
using System.Collections.Concurrent;

namespace Ads.Automation.Infrastructure.SDK.Clients
{
    public class RestClientFactory
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ConcurrentDictionary<string, RestClient> _clients;
        private static Func<RestClientOptions>? _clientOptionsAction;

        static RestClientFactory()
        {
            _clients = new ConcurrentDictionary<string, RestClient>();
        }

        public static RestClient Get(string url)
        {
            ArgumentCheck.NotNullOrWhiteSpace(url, nameof(url));
            if (_clients.ContainsKey(url))
            {
                return _clients[url];
            }

            var client = NewRestClient(url);
            if (_clients.TryAdd(url, client))
            {
                return client;
            }

            return _clients.ContainsKey(url) ? _clients[url] : NewRestClient(url);
        }

        public static void Configure(Func<RestClientOptions> optionsAction)
        {
            _clientOptionsAction = optionsAction;
        }

        private static RestClient NewRestClient(string urlBase)
        {
            RestClientOptions options;
            if (_clientOptionsAction == null)
                options = new RestClientOptions(urlBase) { MaxTimeout = 5 * 60 * 1000 };
            else
            {
                options = _clientOptionsAction.Invoke();
                options.BaseUrl = new Uri(urlBase);
            }

            var client = new RestClient(options);

            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static RestClient Get(Uri uri)
        {
            return Get($"{uri.Scheme}://{uri.Host}");
        }

    }
}
