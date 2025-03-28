using System.Text.Json.Serialization;

namespace EzCad.Extensions.Discord.Models;

/// <summary>
/// Represents the pso
/// </summary>
public class DiscordConfiguration
{
    [JsonPropertyName("isEnabled")] public bool IsEnabled { get; set; }
    [JsonPropertyName("autoJoinServer")] public bool ShouldAutoJoinServer { get; set; } = false;
    [JsonPropertyName("serverId")] public string ServerId { get; set; } = "ENTER_SERVER_ID";
    [JsonPropertyName("botToken")] public string BotToken { get; set; } = "ENTER_BOT_TOKEN";
    [JsonPropertyName("clientId")] public string ClientId { get; set; } = "ENTER_CLIENT_ID";
    [JsonPropertyName("secret")] public string Secret { get; set; } = "ENTER_OAUTH_SECRET";
    [JsonPropertyName("redirectDomain")] public string RedirectDomain { get; set; } = "ENTER_MAIN_WEBSITE_DOMAIN";
}