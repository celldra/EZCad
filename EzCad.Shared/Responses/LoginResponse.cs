using System.Text.Json.Serialization;

namespace EzCad.Shared.Responses;

public class LoginResponse : BaseResponse
{
    [JsonPropertyName("token")] public string Token { get; set; }
}