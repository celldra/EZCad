using System.Text.Json.Serialization;
using EzCad.Database.Entities;

namespace EzCad.Shared.Models;

/// <summary>
///     Represents a mapped object extension to the regular configuration database entity with the added Discord options
///     for the frontend. It also includes a few useful methods too.
/// </summary>
public class FrontendConfiguration : BaseEntity
{
    /// <summary>
    ///     The primary HEX color code to use for buttons and other components
    /// </summary>
    [JsonPropertyName("primaryHexColor")]
    public string PrimaryHexColor { get; set; } = "#B4656F";

    /// <summary>
    ///     The name of the server EZCad is being used for
    /// </summary>
    [JsonPropertyName("serverName")]
    public string ServerName { get; set; } = "EZCad";

    /// <summary>
    ///     The CitizenFX connect URL which should initiate a connection with the server on a specified game (RedM/FiveM)
    /// </summary>
    [JsonPropertyName("connectUrl")]
    public string ConnectUrl { get; set; } = "https://cfx.re";

    /// <summary>
    ///     The unit of currency to use for formatting monetary values, this is by default the USD
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "$";

    /// <summary>
    ///     Whether Discord OAuth2 integration is enabled
    /// </summary>
    [JsonPropertyName("isDiscordEnabled")]
    public bool IsDiscordEnabled { get; set; }

    /// <summary>
    ///     The Discord OAuth2 redirect URL for the regular linking flow
    /// </summary>
    [JsonPropertyName("discordRedirectUrl")]
    public string? DiscordRedirectUrl { get; set; }

    /// <summary>
    ///     The Discord OAuth2 redirect URL for the login flow
    /// </summary>
    [JsonPropertyName("discordLoginRedirectUrl")]
    public string? DiscordLoginRedirectUrl { get; set; }

    /// <summary>
    ///     Formats an amount of currency to a string with the specified unit of currency in the configuration
    /// </summary>
    /// <param name="currency">The amount to format</param>
    /// <returns>The formatted string that should look something like $12,34.56</returns>
    public string FormatCurrency(float currency)
    {
        return $"{Currency}{currency:0,0.00}";
    }

    /// <summary>
    ///     Formats an amount of currency to a string with the specified unit of currency in the configuration
    /// </summary>
    /// <param name="currency">The amount to format</param>
    /// <returns>The formatted string that should look something like $12,34.56</returns>
    public string FormatCurrency(double currency)
    {
        return $"{Currency}{currency:0,0.00}";
    }
}