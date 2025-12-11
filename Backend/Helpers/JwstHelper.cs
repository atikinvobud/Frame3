using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class JwstHelper
    {
        private readonly string host;
        private readonly string key;
        private readonly string? email;
        private readonly HttpClient httpClient;

        public JwstHelper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            host = (Environment.GetEnvironmentVariable("JWST_HOST") ?? "https://api.jwstapi.com").TrimEnd('/');
            key = Environment.GetEnvironmentVariable("JWST_API_KEY") ?? string.Empty;
            email = Environment.GetEnvironmentVariable("JWST_EMAIL");
        }

        public async Task<Dictionary<string, object>> GetAsync(string path, Dictionary<string, string>? query = null)
        {
            var url = $"{host}/{path.TrimStart('/')}";
            if (query != null && query.Count > 0)
            {
                var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var kv in query) queryString[kv.Key] = kv.Value;
                url += url.Contains("?") ? "&" + queryString : "?" + queryString;
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-api-key", key);
            if (!string.IsNullOrWhiteSpace(email))
                request.Headers.Add("email", email);

            try
            {
                var response = await httpClient.SendAsync(request);
                var raw = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    var json = JsonSerializer.Deserialize<Dictionary<string, object>>(raw, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return json ?? new Dictionary<string, object>();
                }
            }
            catch
            {
            }

            return new Dictionary<string, object>();
        }

        public static string? PickImageUrl(Dictionary<string, object> item)
        {
            string[] keys = { "thumbnail", "thumbnailUrl", "image", "img", "url", "href", "link", "s3_url", "file_url" };

            foreach (var key in keys)
            {
                if (item.TryGetValue(key, out var value) && value is string s)
                {
                    var u = s.Trim();
                    if ((u.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                         u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) &&
                        (u.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                         u.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                         u.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                        return u;

                    if (u.StartsWith("/") && (u.EndsWith(".jpg") || u.EndsWith(".jpeg") || u.EndsWith(".png")))
                        return "https://api.jwstapi.com" + u;
                }
            }

            foreach (var v in item.Values)
            {
                if (v is Dictionary<string, object> nestedDict)
                {
                    var u = PickImageUrl(nestedDict);
                    if (u != null) return u;
                }
            }

            return null;
        }
    }
}
