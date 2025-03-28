using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using EzCad.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace EzCad.Web.Providers;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    private const string JwtAuthType = "jwtAuthType";
    private const string JwtLocalStorageKey = "auth_token";
    private readonly AuthenticationState _anonymous;
    private readonly TokenValidationParameters _appTokenValidationParameters;
    private readonly IBackendConfigurationService _backendConfigurationService;
    private readonly IHttpClientFactory _factory;
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<JwtAuthStateProvider> _logger;

    public JwtAuthStateProvider(ILocalStorageService localStorage, IHttpClientFactory factory,
        ILogger<JwtAuthStateProvider> logger, IBackendConfigurationService backendConfigurationService)
    {
        _localStorage = localStorage;
        _factory = factory;
        _logger = logger;
        _backendConfigurationService = backendConfigurationService;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        _appTokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            ValidateAudience = true,
            SignatureValidator = (token, _) =>
            {
                var jwt = new JwtSecurityToken(token);

                return jwt;
            },
            RequireAudience = true,
            RequireExpirationTime = true,
            ValidAudiences = new[] {"EZCad"},
            ValidateIssuer = true,
            ValidIssuer = _backendConfigurationService.Configuration.JwtIssuer,
            RequireSignedTokens = false,
            ValidateLifetime = true,
            ValidAlgorithms = new[] {"HS256"},
            ValidateTokenReplay = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    }

    private bool TryReadAppToken(string jwtToken, out JwtSecurityToken? token)
    {
        try
        {
            jwtToken = jwtToken.TrimStart('"')
                .TrimEnd('"');

            token = null;

            if (!_handler.CanReadToken(jwtToken)) return false;
            _handler.ValidateToken(jwtToken, _appTokenValidationParameters, out _);

            token = _handler.ReadJwtToken(jwtToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Read JWT token in state provider failed");
            token = null;
            return false;
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(JwtLocalStorageKey);

        if (string.IsNullOrWhiteSpace(token) || !TryReadAppToken(token, out var jwtSecurityToken)) return _anonymous;

        _factory.CreateClient("api").SetAuthorizationHeader(token);

        return new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(jwtSecurityToken?.Claims, JwtAuthType)));
    }

    internal void NotifyUserAuthentication(string token)
    {
        if (!TryReadAppToken(token, out var jwtSecurityToken)) return;

        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(jwtSecurityToken?.Claims,
                JwtAuthType));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    internal void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}