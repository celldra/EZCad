using EzCadSync.Api.Models;
using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class IdentityResponse : BaseResponse
{
    [JsonProperty("vehicles")] public Vehicle[] Vehicles { get; set; }
    [JsonProperty("identity")] public Identity Identity { get; set; }
}