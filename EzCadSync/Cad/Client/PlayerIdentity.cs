using EzCadSync.Client.Models;
using Newtonsoft.Json;

namespace EzCadSync.Client;

public class PlayerIdentity : Identity
{
    [JsonProperty("name")] public string Name { get; set; }
}