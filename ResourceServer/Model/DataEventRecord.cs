using System.Text.Json.Serialization;

namespace ResourceServer.Model;

public class DataEventRecord
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}
