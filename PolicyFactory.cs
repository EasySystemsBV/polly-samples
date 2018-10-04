using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using Polly.Timeout;
using Refit;

namespace PollyDemo
{
    public class PolicyFactory
    {
        private const int NUMBER_TIMES_RETRY_AUTOCOMPLETE = 2;
        private const int TIME_OUT_IN_SECONDS = 5;
        private const int MINUTES_TO_CACHE_AUTOCOMPLETE_RESULTS = 5;
        private readonly HttpStatusCode[] httpStatusCodesWorthRetrying =
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        };

        public PolicyFactory()
        {
            InitializePolicies();
        }

        public IAsyncPolicy<AutocompleteResult> ApiCallPolicyAutocomplete { get; private set; }

        private void InitializePolicies()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
            var cachePolicy = Policy.CacheAsync(
                memoryCacheProvider,
                TimeSpan.FromMinutes(MINUTES_TO_CACHE_AUTOCOMPLETE_RESULTS));

            var timeoutPolicy = Policy.TimeoutAsync(TIME_OUT_IN_SECONDS);

            var retryPolicy = Policy
                .Handle<ApiException>(ex => httpStatusCodesWorthRetrying.Contains(ex.StatusCode))
                .RetryAsync(NUMBER_TIMES_RETRY_AUTOCOMPLETE);

            var fallbackPolicyAutocomplete = Policy<AutocompleteResult>
                .Handle<TimeoutRejectedException>()
                .Or<ApiException>()
                .Or<WebException>()
                .Or<HttpRequestException>()
                .FallbackAsync(new AutocompleteResult
                {
                    Status = GoogleMapsConstants.STATUS_OFFLINE,
                    Predictions = new List<string>()
                });
            ApiCallPolicyAutocomplete = fallbackPolicyAutocomplete.WrapAsync(cachePolicy).WrapAsync(timeoutPolicy).WrapAsync(retryPolicy);
        }
    }
}
