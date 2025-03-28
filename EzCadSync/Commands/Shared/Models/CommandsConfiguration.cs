using System.Collections.Generic;
using Newtonsoft.Json;

namespace GallagherCommands.Configuration.Models;

public class CommandsConfiguration
{
    [JsonProperty("respawn_interval_seconds")]
    public int RespawnInterval { get; set; }

    [JsonProperty("disabled_commands")] public List<string> DisabledCommands { get; set; }
    [JsonProperty("respawn_location")] public CoOrdinates RespawnCoOrdinates { get; set; }

    [JsonProperty("announcement_duration")]
    public int AnnouncementDuration { get; set; }

    [JsonProperty("enable_join_spawn")] public bool EnableJoinSpawn { get; set; } = true;

    [JsonIgnore] public bool EnableNameTags { get; set; } = false;
    [JsonProperty("default_aop")] public string DefaultAop { get; set; }
    [JsonProperty("ragdoll_key")] public char RagdollKey { get; set; }
    [JsonProperty("enable_wasted_screen")] public bool EnableWastedScreen { get; set; }
    [JsonProperty("strings")] public Dictionary<string, string> Strings { get; set; }
}