using Newtonsoft.Json;

namespace EzCadSync.Shared.Responses;

public abstract class BaseResponse
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("message")] public string Message { get; set; }
}