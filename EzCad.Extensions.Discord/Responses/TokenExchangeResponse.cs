using System.Text.Json.Serialization;

namespace EzCad.Extensions.Discord.Responses;

/// <summary>
///     Represents a successful OAuth2 token exchange response
/// </summary>
public class TokenExchangeResponse
{
    public TokenExchangeResponse(string accessToken, string tokenType, long expiresIn, string refreshToken,
        string scope)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        RefreshToken = refreshToken;
        Scope = scope;
    }

    /// <summary>
    ///     The OAuth2 access token to present to Discord's API to authenticate the user
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; }

    /// <summary>
    ///     The type of token that was generated
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; }

    /// <summary>
    ///     The time in seconds after the current date that the access token expires
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; }

    /// <summary>
    ///     The token to use when refreshing the access token
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; }

    /// <summary>
    ///     The scopes the access token allows usage of
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; }
}