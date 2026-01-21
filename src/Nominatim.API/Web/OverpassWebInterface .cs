using Nominatim.API.Interfaces;
using Nominatim.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nominatim.API.Web
{
    public sealed class OverpassWebInterface : IOverpassWebInterface
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OverpassWebInterface(IHttpClientFactory http)
        {
            _httpClientFactory = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<OverpassResponse> QueryAsync(string overpassQl, CancellationToken ct = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            AddUserAgent(httpClient);
            // Overpass "interpreter" erwartet form-encoded "data="
            using var content = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("data", overpassQl) });

            using var resp = await httpClient.PostAsync("", content, ct).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();

            var result = await resp.Content.ReadFromJsonAsync<OverpassResponse>(cancellationToken: ct)
                         .ConfigureAwait(false);

            return result ?? new OverpassResponse();
        }

        private static void AddUserAgent(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("pat2381.Nominatim.API", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }
    }
}
