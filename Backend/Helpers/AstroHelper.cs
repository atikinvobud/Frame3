using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Backend.Services
{
    public class AstroHelper
    {
        private readonly HttpClient http;
        private readonly string appId;
        private readonly string secret;

        private const string ASTRO_HOST = "https://api.astronomyapi.com";

        public AstroHelper(IConfiguration cfg, IHttpClientFactory factory)
        {
            http = factory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(25);

            appId = cfg["ASTRO_APP_ID"]
                ?? throw new InvalidOperationException("ASTRO_APP_ID missing");

            secret = cfg["ASTRO_APP_SECRET"]
                ?? throw new InvalidOperationException("ASTRO_APP_SECRET missing");
        }

        public async Task<object> GetEventsAsync(string body, double lat, double lon, int days)
        {
            days = Math.Clamp(days, 1, 366);

            var fromDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var toDate = DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");

            var query =
                $"latitude={lat}&longitude={lon}&elevation=0&from_date={fromDate}&to_date={toDate}&time=00:00:00&output=table";

            var url = $"{ASTRO_HOST}/api/v2/bodies/events/{body}?{query}";

            string authRaw = $"{appId}:{secret}";
            string authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authRaw));
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authBase64);

            http.DefaultRequestHeaders.UserAgent.ParseAdd("monolith-iss/1.0");
            http.DefaultRequestHeaders.Remove("Origin");
            http.DefaultRequestHeaders.Add("Origin", "http://localhost");

            var response = await http.GetAsync(url);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new
                {
                    error = $"HTTP {(int)response.StatusCode}",
                    raw
                };
            }

            try
            {
                return JsonSerializer.Deserialize<object>(raw)!;
            }
            catch
            {
                return new { raw };
            }
        }
    }
}
