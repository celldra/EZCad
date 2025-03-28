using System.Text.Json.Serialization;

namespace EzCad.Extensions.Discord.Models;

/// <summary>
/// Represents a user present on Discord
/// </summary>
public class DiscordUser
{
    [JsonConstructor]
    public DiscordUser(string id, string userName, string discriminator)
    {
        Id = id;
        UserName = userName;
        Discriminator = discriminator;
    }

    /// <summary>
    ///     The ID of the user
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    ///     The username of the user
    /// </summary>
    [JsonPropertyName("username")]
    public string UserName { get; }

    /// <summary>
    ///     The users discriminator, this is the 4 numbers after the hashtag in their username
    /// </summary>
    [JsonPropertyName("discriminator")]
    public string Discriminator { get; }
}