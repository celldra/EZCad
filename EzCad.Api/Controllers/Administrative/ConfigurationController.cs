using System.Web;
using AutoMapper;
using EzCad.Extensions.Discord;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Models;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers.Administrative;

[ApiController]
[Route("/config/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class ConfigurationController : ControllerBase
{
    private readonly IFrontendConfigurationService _configurationService;
    private readonly IMapper _mapper;
    private readonly IBackendConfigurationService _backendConfigurationService;
    private readonly IWebHostEnvironment _environment;

    public ConfigurationController(IFrontendConfigurationService configurationService, IWebHostEnvironment environment,
        IBackendConfigurationService backendConfigurationService, IMapper mapper)
    {
        _configurationService = configurationService;
        _environment = environment;
        _backendConfigurationService = backendConfigurationService;
        _mapper = mapper;
    }

    [Route("/config")]
    [HttpGet]
    public async Task<IActionResult> GetConfiguration(CancellationToken cancellationToken)
    {
        var configuration = await _configurationService.GetConfigurationAsync(false, cancellationToken);

        if (configuration is null)
            throw new InvalidOperationException(
                "Race condition has been met with configuration, please open an issue on GitHub");

        var frontendConfiguration = _mapper.Map<FrontendConfiguration>(configuration);
        frontendConfiguration.IsDiscordEnabled =
            _backendConfigurationService.Configuration.DiscordConfiguration.IsEnabled;

        if (!frontendConfiguration.IsDiscordEnabled) return Ok(frontendConfiguration);

        var clientId = _backendConfigurationService.Configuration.DiscordConfiguration.ClientId;

        var redirectLogin =
            DiscordService.PrepareRedirectUrl(
                _backendConfigurationService.Configuration.DiscordConfiguration.RedirectDomain, true);
        var redirect =
            DiscordService.PrepareRedirectUrl(
                _backendConfigurationService.Configuration.DiscordConfiguration.RedirectDomain);

        frontendConfiguration.DiscordRedirectUrl =
            $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={HttpUtility.UrlEncode(redirect)}&response_type=code&scope=guilds.join%20identify";

        frontendConfiguration.DiscordLoginRedirectUrl =
            $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={HttpUtility.UrlEncode(redirectLogin)}&response_type=code&scope=guilds.join%20identify";

        return Ok(frontendConfiguration);
    }

    [Route("/config")]
    [HttpPut]
    [Authorize(Roles = RoleValues.Administrator)]
    public async Task<IActionResult> UpdateConfiguration([FromBody] ConfigurationForm form,
        CancellationToken cancellationToken)
    {
        await _configurationService.UpdateConfigurationAsync(form.ServerName, form.HexColor, form.ServerConnectUrl,
            form.Currency, cancellationToken);

        return NoContent();
    }

    [Route("/config/logo")]
    [HttpGet]
    [Produces("image/png")]
    public Task<IActionResult> GetLogoAsync()
    {
        var path = Path.Combine(_environment.ContentRootPath, "uploads", "logo.png");
        if (!System.IO.File.Exists(path))
            return Task.FromResult<IActionResult>(NotFound());
        
        var stream = new FileStream(path, FileMode.Open);

        return Task.FromResult<IActionResult>(File(stream, "image/png", true));
    }

    [Route("/config/logo")]
    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateLogoAsync([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        const int maxFileSize = 1024 * 1024 * 15;

        if (file.Length is 0 or > maxFileSize)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "The new image is too large, please make sure it is not bigger than 15 MB (megabytes)"
            });

        try
        {
            var directory = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var path = Path.Combine(_environment.ContentRootPath, "uploads", "logo.png");

            await using var fs = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fs, cancellationToken);
        }
        catch (IOException)
        {
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message =
                    "Failed to write the new icon to storage, check that there is enough space or correct permissions in the content root directory"
            });
        }


        return NoContent();
    }
}