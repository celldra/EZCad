using System.Text.Json.Serialization;

namespace EzCad.Api.Models;

public class ValidationError
{
    [JsonPropertyName("field")] public string? Field { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }
}