using EzCad.Database.Entities;
using EzCad.Extensions.Discord.Interfaces;
using EzCad.Services.Interfaces;
using EzCad.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class DiscordController : ControllerBase
{
    private readonly IDiscordService _discordService;
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;
    private readonly IBackendConfigurationService _backendConfigurationService;
    private readonly IJwtAuthenticationService<User> _jwtAuthenticationService;

    public DiscordController(IDiscordService discordService, UserManager<User> userManager, IUserService userService,
        IJwtAuthenticationService<User> jwtAuthenticationService,
        IBackendConfigurationService backendConfigurationService)
    {
        _discordService = discordService;
        _userManager = userManager;
        _userService = userService;
        _jwtAuthenticationService = jwtAuthenticationService;
        _backendConfigurationService = backendConfigurationService;
    }

    [Route("/external/discord")]
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> UnlinkDiscordAsync(CancellationToken cancellationToken)
    {
        // Get the user
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        user.DiscordId = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [Route("/external/discord/{token:required}")]
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> ExternalDiscordLinkAsync(string token,
        CancellationToken cancellationToken)
    {
        // Get the user
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        // We've got to exchange the token and fetch their Discord profile
        var exchange = await _discordService.ExchangeTokenAsync(token, false, cancellationToken);

        // Now we've exchanged, we need to get their profile
        var profile = await _discordService.GetUserProfileAsync(exchange.AccessToken, cancellationToken);
        if (profile is null)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "Unable to get Discord profile"
            });

        // If server joining is enabled, join the server
        if (_backendConfigurationService.Configuration.DiscordConfiguration.ShouldAutoJoinServer)
            await _discordService.JoinServerAsync(exchange.AccessToken, ulong.Parse(profile.Id), cancellationToken);

        // Now update their user
        user.DiscordId = profile.Id;

        await _userManager.UpdateAsync(user);

        return Ok(new ErrorResponse
        {
            Success = true,
            Message = "Account has been linked"
        });
    }

    [Route("/auth/external/discord/{token:required}")]
    [HttpPut]
    public async Task<IActionResult> ExternalDiscordAuthenticationAsync(string token,
        CancellationToken cancellationToken)
    {
        // We've got to exchange the token and fetch their Discord profile
        var exchange = await _discordService.ExchangeTokenAsync(token, true, cancellationToken);

        // Now we've exchanged, we need to get their profile
        var profile = await _discordService.GetUserProfileAsync(exchange.AccessToken, cancellationToken);
        if (profile is null)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "Unable to get Discord profile"
            });

        // We've got their profile, now find the linked user in the database
        var user = await _userService.GetUserByDiscordIdAsync(profile.Id, cancellationToken);
        if (user is null)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "No CAD user was found under the account"
            });

        // Now we can finish up and generate a JWT for them
        var authToken = _jwtAuthenticationService.GetToken(user);

        return Ok(new LoginResponse
        {
            Success = true,
            Token = authToken
        });
    }
}