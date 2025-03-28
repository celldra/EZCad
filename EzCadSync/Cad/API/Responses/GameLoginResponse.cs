using System.Collections.Generic;
using EzCadSync.Api.Models;
using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class GameLoginResponse : BaseResponse
{
    [JsonProperty("vehicles")] public List<Vehicle> Vehicles { get; set; }

    [JsonProperty("identity")] public Identity Identity { get; set; }

    [JsonProperty("profile")] public UserProfile Profile { get; set; }

    [JsonProperty("sessionId")] public string SessionId { get; set; }
}