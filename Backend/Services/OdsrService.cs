using System.Text.Json;
using Backend.Models.Entities;
using Backend.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class OsdrService : IOsdrService
{
    private readonly IHttpClientFactory httpFactory;
    private readonly ISpaceCacheRepository spaceRepository;
    private readonly IOsdrRepository repository;
    private readonly string nasaUrl;
    private readonly ILogger<OsdrService> logger;

    public OsdrService(IHttpClientFactory httpFactory,ISpaceCacheRepository spaceRepository,IOsdrRepository repository,IOptions<ApiUrls> urls,ILogger<OsdrService> logger)
    {
        this.httpFactory = httpFactory;
        this.repository = repository;
        this.spaceRepository = spaceRepository;
        this.logger = logger;
        nasaUrl = urls.Value.OsdrDatasetUrl;
    }

    public async Task<int> FetchAndStoreAsync(CancellationToken ct = default)
    {
        var client = httpFactory.CreateClient();

        try
        {
            logger.LogInformation("[OSDR] Fetching from {url}", nasaUrl);

            var res = await client.GetStringAsync(nasaUrl, ct);

            await spaceRepository.AddAsync(new SpaceCache
            {
                Source = "osdr_count",
                Payload = res,      
                FetchedAt = DateTime.UtcNow
            });

            using var doc = JsonDocument.Parse(res);
            JsonElement root = doc.RootElement;

            var elements = new List<(JsonElement El, string? KeyHint)>();

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in root.EnumerateArray())
                    elements.Add((el, null));
            }
            else if (root.TryGetProperty("items", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in arr.EnumerateArray())
                    elements.Add((el, null));
            }
            else if (root.TryGetProperty("results", out var arr2) && arr2.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in arr2.EnumerateArray())
                    elements.Add((el, null));
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in root.EnumerateObject())
                    elements.Add((prop.Value, prop.Name));
            }
            else
            {
                elements.Add((root, null));
            }

            var list = new List<OsdrItem>(elements.Count);

            foreach (var (el, keyHint) in elements)
            {
                string raw = el.GetRawText();

                string? datasetId =
                    PickString(el, new[] { "dataset_id", "id", "uuid", "studyId", "accession", "osdr_id" });

                if (string.IsNullOrWhiteSpace(datasetId) && !string.IsNullOrWhiteSpace(keyHint))
                    datasetId = keyHint;

                datasetId ??= Guid.NewGuid().ToString("N");

                var updatedAt =
                    PickDate(el, new[] { "updated", "updated_at", "modified", "lastUpdated", "timestamp" })
                    ?? DateTime.UtcNow;

                var model = new OsdrItem
                {
                    DatasetId = datasetId,
                    Title = datasetId,
                    Status = "Ok",
                    UpdatedAt = updatedAt,
                    Raw = raw,
                    InsertedAt = DateTime.UtcNow
                };

                list.Add(model);
            }


            int written = await repository.UpsertManyAsync(list, ct);

            logger.LogInformation("[OSDR] Saved {count} items and raw JSON cached", written);

            return written;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[OSDR] Error fetching or storing OSDR");
            throw;
        }
    }
    private static string? PickString(JsonElement el, string[] keys)
    {
        foreach (var k in keys)
        {
            if (!el.TryGetProperty(k, out var v))
                continue;

            if (v.ValueKind == JsonValueKind.String)
            {
                var s = v.GetString();
                if (!string.IsNullOrWhiteSpace(s))
                    return s;
            }
            else if (v.ValueKind == JsonValueKind.Number)
                return v.GetRawText();
        }
        return null;
    }
    private static DateTime? PickDate(JsonElement el, string[] keys)
    {
        foreach (var k in keys)
        {
            if (!el.TryGetProperty(k, out var v))
                continue;

            if (v.ValueKind == JsonValueKind.String && DateTime.TryParse(v.GetString(), out var dt))
            {
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }
            else if (v.ValueKind == JsonValueKind.Number && v.TryGetInt64(out var epoch))
            {
                return DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
            }
        }
        return null;
    }
}
