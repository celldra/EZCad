using EzCad.Extensions.Discord.Exceptions;
using EzCad.Extensions.Discord.Models;
using EzCad.Extensions.Discord.Responses;

namespace EzCad.Extensions.Discord.Interfaces;

/// <summary>
///     Abstraction interface for the Discord service
/// </summary>
public interface IDiscordService
{
    /// <summary>
    ///     Holds the configuration for the current service
    /// </summary>
    DiscordConfiguration Configuration { get; }

    /// <summary>
    ///     Joins a Discord server provided that the bot the token is associated with an account that is in the server that is
    ///     specified in the configuration and that the feature is enabled
    /// </summary>
    /// <param name="accessToken">The bearer access token</param>
    /// <param name="userId">The ID of the user to add into the server</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="DiscordException">
    ///     Thrown when the link was not successful for one reason or another
    /// </exception>
    Task JoinServerAsync(string accessToken, ulong userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a users Discord profile from a bearer access token
    /// </summary>
    /// <param name="accessToken">The bearer access token</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="DiscordException">
    ///     Thrown when the Discord API sends a non-OK status code or a response that
    ///     could not be deserialized
    /// </exception>
    /// <returns>The Discord profile</returns>
    Task<DiscordUser?> GetUserProfileAsync(string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Exchanges a Discord OAuth2 token to a fully qualified Discord bearer token which can be used to access the
    ///     requested scopes on the users account
    /// </summary>
    /// <param name="token">The OAuth2 token</param>
    /// <param name="isLogin">Whether the exchange is for a login request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="DiscordException">
    ///     Thrown when the Discord API sends a non-OK status code or a response that
    ///     could not be deserialized
    /// </exception>
    /// <returns>The response containing details about the returned bearer token</returns>
    Task<TokenExchangeResponse> ExchangeTokenAsync(string token, bool isLogin = false,
        CancellationToken cancellationToken = default);
}