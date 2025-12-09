using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

namespace Backend.Controllers;

[ApiController]
[Route("jwst")]
public class JwstController : ControllerBase
{
    private readonly JwstHelper jwstHelper;

    public JwstController(JwstHelper jwstHelper)
    {
        this.jwstHelper = jwstHelper;
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] string source = "jpg",
        [FromQuery] string? suffix = null,
        [FromQuery] string? program = null,
        [FromQuery] string? instrument = null,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 24)
    {
        try
        {
            string path = source.ToLower() switch
            {
                "suffix" when !string.IsNullOrWhiteSpace(suffix) => $"all/suffix/{HttpUtility.UrlEncode(suffix)}",
                "program" when !string.IsNullOrWhiteSpace(program) => $"program/id/{HttpUtility.UrlEncode(program)}",
                _ => "all/type/jpg"
            };

            var query = new Dictionary<string, string>
            {
                ["page"] = page.ToString(),
                ["perPage"] = perPage.ToString()
            };

            var data = await jwstHelper.GetAsync(path, query);

            if (!string.IsNullOrWhiteSpace(instrument) && data.TryGetValue("body", out var bodyObj) &&
                bodyObj is JsonElement bodyEl && bodyEl.ValueKind == JsonValueKind.Array)
            {
                var items = new List<Dictionary<string, object>>();
                foreach (var itemEl in bodyEl.EnumerateArray())
                {
                    if (itemEl.ValueKind != JsonValueKind.Object) continue;
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(itemEl.GetRawText())!;
                    if (dict.TryGetValue("details", out var detailsObj) && detailsObj is JsonElement detailsEl &&
                        detailsEl.TryGetProperty("instruments", out var instrumentsEl))
                    {
                        var instList = instrumentsEl.EnumerateArray()
                            .Select(i => i.GetProperty("instrument").GetString()?.ToUpper() ?? "")
                            .ToList();
                        if (!instList.Contains(instrument.ToUpper())) continue;
                    }
                    items.Add(dict);
                    if (items.Count >= perPage) break;
                }
                data["body"] = items;
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

}

