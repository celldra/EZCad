using Newtonsoft.Json;

namespace EzCadSync.Api.Models;

public class PlayerIdentity : Identity
{
    [JsonProperty("name")] public string Name { get; set; }
}