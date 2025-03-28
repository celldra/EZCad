using Newtonsoft.Json;

namespace EzCadSync.Api.Models;

public class ValidationError
{
    [JsonProperty("field")] public string? Field { get; set; }

    [JsonProperty("message")] public string Message { get; set; }
}