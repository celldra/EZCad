using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EzCad.Extensions.Discord.Exceptions;
using EzCad.Extensions.Discord.Interfaces;
using EzCad.Extensions.Discord.Models;
using EzCad.Extensions.Discord.Responses;

namespace EzCad.Extensions.Discord;

/// <summary>
///     Handles all Discord related functions including OAuth2 exchanges
/// </summary>
public class DiscordService : IDiscordService
{
    #region Services

    private readonly IHttpClientFactory _factory;

    #endregion

    /// <summary>
    ///     Holds the configuration for the current service
    /// </summary>
    public DiscordConfiguration Configuration { get; }

    public DiscordService(IHttpClientFactory factory, DiscordConfiguration configuration)
    {
        _factory = factory;
        Configuration = configuration;
    }

    /// <summary>
    ///     Constructs a Discord API prepared <see cref="HttpClient" /> from the injected <see cref="IHttpClientFactory" />
    /// </summary>
    /// <returns>The newly created <see cref="HttpClient" /></returns>
    private HttpClient ConstructClient()
    {
        return _factory.CreateClient("discordapi");
    }

    /// <summary>
    ///     Prepares a redirect URL string into the correct format depending on the login status
    /// </summary>
    /// <param name="redirectUrl">The base URL to use</param>
    /// <param name="isLogin">Whether the URL should direct to Discord OAuth2 login or regular linking</param>
    /// <returns>The fully qualified URL</returns>
    public static string PrepareRedirectUrl(string redirectUrl, bool isLogin = false)
    {
        // Trim any trailing slashes on the end
        redirectUrl = redirectUrl.TrimEnd('/');

        return $"{redirectUrl}{(isLogin ? "/login/discord" : "/user/external/discord-callback")}";
    }

    /// <summary>
    ///     Handles a <see cref="HttpResponseMessage" /> and parses it back to a readable model
    /// </summary>
    /// <param name="message">The message to parse</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of object to deserialize back to</typeparam>
    /// <returns>The object of type specified in the type parameter</returns>
    /// <exception cref="DiscordException">
    ///     Thrown when the Discord API sends a non-OK status code or a response that
    ///     could not be deserialized
    /// </exception>
    private static async Task<T> HandleResponseAsync<T>(HttpResponseMessage message,
        CancellationToken cancellationToken = default)
    {
        var debugResponseContent = await message.Content.ReadAsStringAsync(cancellationToken);
        if (!message.IsSuccessStatusCode)
            throw new DiscordException($"{message.StatusCode} status code was received from the Discord API");
        
        var responseStream = await message.Content.ReadAsStreamAsync(cancellationToken);
        var json = await JsonSerializer.DeserializeAsync<T>(responseStream, cancellationToken: cancellationToken);

        if (json is null)
            throw new DiscordException(
                "Despite the Discord API sending a 200 (OK) status code, the response could not be understood");

        return json;
    }

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
    public async Task JoinServerAsync(string accessToken, ulong userId,
        CancellationToken cancellationToken = default)
    {
        // Just return true if they haven't got the function enabled
        if (!Configuration.ShouldAutoJoinServer) return;

        // Build a client and authorize with the bot token
        using var client = ConstructClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TokenType.Bot, Configuration.BotToken);

        var content = new StringContent(JsonSerializer.Serialize(new
        {
            access_token = accessToken
        }), Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Put, $"guilds/{Configuration.ServerId}/members/{userId}");
        request.Content = content;

        using var response = await client.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new DiscordException($"Discord link has failed: {response.StatusCode}");
    }

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
    public async Task<DiscordUser?> GetUserProfileAsync(string accessToken,
        CancellationToken cancellationToken = default)
    {
        // Create the HttpClient
        using var client = ConstructClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType.Bearer, accessToken);

        // Prepare the request message
        using var request = new HttpRequestMessage(HttpMethod.Get, "users/@me");

        // Send the request and handle the response back
        using var response = await client.SendAsync(request, cancellationToken);
        return await HandleResponseAsync<DiscordUser>(response, cancellationToken);
    }

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
    public async Task<TokenExchangeResponse> ExchangeTokenAsync(string token, bool isLogin = false,
        CancellationToken cancellationToken = default)
    {
        // Get the redirect URL depending on whether it's a login or not and get a HttpClient
        var redirectUrl = PrepareRedirectUrl(Configuration.RedirectDomain, isLogin);
        using var client = ConstructClient();

        // Prepare the request body
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"client_id", Configuration.ClientId.ToString()},
            {"client_secret", Configuration.Secret},
            {"grant_type", "authorization_code"},
            {"code", token},
            {"redirect_uri", redirectUrl}
        });

        // Prepare the request message
        using var request = new HttpRequestMessage(HttpMethod.Post, "oauth2/token");
        request.Content = body;

        // Send the request and handle the response back
        using var response = await client.SendAsync(request, cancellationToken);
        return await HandleResponseAsync<TokenExchangeResponse>(response, cancellationToken);
    }
}