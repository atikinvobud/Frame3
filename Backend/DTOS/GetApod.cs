using System.Text.Json.Serialization;

public class ApodDto
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("media_type")]
    public string MediaType { get; set; } = "";

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = "";

    [JsonPropertyName("service_version")]
    public string ServiceVersion { get; set; } = "";
    public string Source { get; set; } = null!;
}
