using Newtonsoft.Json;

namespace EzCadSync.Shared.Models;

public class DiscordRichPresenceConfiguration
{
    [JsonProperty("is_enabled")] public bool IsEnabled { get; set; }
    [JsonProperty("client_id")] public string ClientId { get; set; }
    [JsonProperty("state")] public string State { get; set; }
    [JsonProperty("action_text")] public string ActionText { get; set; }
    [JsonProperty("action_url")] public string ActionUrl { get; set; }
    [JsonProperty("asset_name")] public string AssetName { get; set; }
}