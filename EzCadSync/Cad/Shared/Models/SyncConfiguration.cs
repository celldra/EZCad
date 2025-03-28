using Newtonsoft.Json;

namespace EzCadSync.Shared.Models;

public class SyncConfiguration
{
    [JsonProperty("api_base_url")] public string ApiBaseUrl { get; set; }
    [JsonProperty("routine_message")] public string RoutineMessage { get; set; }

    [JsonProperty("cad_not_linked_message")]
    public string CadNotLinkedMessage { get; set; }

    [JsonProperty("manually_shutdown_loadscreen")]
    public bool ShouldManuallyShutdownLoadscreen { get; set; }

    [JsonProperty("server_name")] public string ServerName { get; set; }

    [JsonProperty("cad_not_linked_card_text")]
    public string CadNotLinkedCardText { get; set; }

    [JsonProperty("discord_url")] public string DiscordUrl { get; set; }
    [JsonProperty("cad_url")] public string CadUrl { get; set; }
    [JsonProperty("rich_presence")] public DiscordRichPresenceConfiguration RichPresenceConfiguration { get; set; }
}