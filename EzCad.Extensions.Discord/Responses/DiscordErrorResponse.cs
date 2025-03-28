using System.Text.Json.Serialization;

namespace EzCad.Extensions.Discord.Responses;

/// <summary>
///     Represents an error returned by the Discord API
/// </summary>
public class DiscordErrorResponse
{
    [JsonConstructor]
    public DiscordErrorResponse(long code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    ///     The error code
    /// </summary>
    [JsonPropertyName("code")]
    public long Code { get; }

    /// <summary>
    ///     The friendly message for the error, this should be used in-favour of any other method as it will be the most
    ///     accurate
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; }
}