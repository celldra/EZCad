using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class UserProfile : BaseResponse
{
    [JsonProperty("userName")] public string UserName { get; set; }

    [JsonProperty("roles")] public string[] Roles { get; set; }
}