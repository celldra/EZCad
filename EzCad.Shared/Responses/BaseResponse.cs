using System.Text.Json.Serialization;

namespace EzCad.Shared.Responses;

public abstract class BaseResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }
}