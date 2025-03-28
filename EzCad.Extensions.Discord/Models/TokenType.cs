namespace EzCad.Extensions.Discord.Models;

/// <summary>
/// Static class containing constant values for different Discord token types
/// </summary>
public static class TokenType
{
    /// <summary>
    ///     Bearer tokens are used for authenticating with an OAuth2 access token
    /// </summary>
    public const string Bearer = "Bearer";

    /// <summary>
    ///     Bot tokens are for authenticating as a bot user
    /// </summary>
    public const string Bot = "Bot";
}