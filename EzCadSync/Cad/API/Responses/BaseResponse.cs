using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public abstract class BaseResponse
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("message")] public string Message { get; set; }
}