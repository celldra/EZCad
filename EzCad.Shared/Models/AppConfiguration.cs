using System.Text.Json.Serialization;
using EzCad.Extensions.Discord.Models;
using EzCad.Shared.Utils;

namespace EzCad.Shared.Models;

public class AppConfiguration
{
    [JsonPropertyName("redisConfiguration")]
    public DatabaseConfiguration RedisConfiguration { get; set; } = new();

    [JsonPropertyName("databaseConfiguration")]
    public DatabaseConfiguration DatabaseConfiguration { get; set; } = new();

    [JsonPropertyName("domains")]
    public string[] Domains { get; set; } =
    {
        "github.com",
        "example.com"
    };

    [JsonPropertyName("discordConfiguration")]
    public DiscordConfiguration DiscordConfiguration { get; set; } = new();

    [JsonPropertyName("apiBaseUrl")] public string ApiBaseUrl { get; set; } = "https://api.my-cad-url.com/";

    [JsonPropertyName("passwordSecret")]
    public string PasswordSecret { get; set; } = RandomExtended.GetRandomString(512);

    [JsonPropertyName("jwtSigningKey")] public string JwtSigningKey { get; set; } = RandomExtended.GetRandomString(128);

    [JsonPropertyName("jwtIssuer")] public string JwtIssuer { get; set; } = "EZCad";
}