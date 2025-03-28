using EzCadSync.Api.Models;
using EzCadSync.Shared.Models;
using Newtonsoft.Json;

namespace EzCadSync.Server;

public class PlayerIdentity : Identity
{
    [JsonProperty("name")] public string Name { get; set; }
}