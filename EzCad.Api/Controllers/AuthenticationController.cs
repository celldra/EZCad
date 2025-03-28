using AutoMapper;
using EzCad.Api.Responses;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Models;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class AuthenticationController : ControllerBase
{
    private readonly IFrontendConfigurationService _frontendConfigurationService;
    private readonly IGameLoginService _gameLoginService;
    private readonly IIdentityService _identityService;
    private readonly IJwtAuthenticationService<User> _jwtAuthentication;
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IVehicleService _vehicleService;

    public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager,
        IJwtAuthenticationService<User> jwtAuthentication, IGameLoginService gameLoginService,
        IIdentityService identityService, IVehicleService vehicleService,
        IFrontendConfigurationService frontendConfigurationService,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtAuthentication = jwtAuthentication;
        _gameLoginService = gameLoginService;
        _identityService = identityService;
        _vehicleService = vehicleService;
        _frontendConfigurationService = frontendConfigurationService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("/auth/register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterForm form, CancellationToken cancellationToken)
    {
        var u = new User
        {
            UserName = form.UserName,
            Email = form.Email
        };

        var result = await _userManager.CreateAsync(u, form.Password);
        if (result.Succeeded)  return StatusCode(201, new CreatedResponse<User>(u));

        return BadRequest(new ErrorResponse
        {
            Success = false,
            Message = result.Convert()
        });
    }

    [HttpPost]
    [Route("/auth/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginForm form, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(form.UserName);
        var configuration = await _frontendConfigurationService.GetConfigurationAsync(false, cancellationToken);
        if (user is null)
            return Unauthorized(new ErrorResponse
            {
                Success = false,
                Message = "Invalid username or password"
            });

        if (await _userManager.IsInRoleAsync(user, RoleValues.Banned))
            return Unauthorized(new ErrorResponse
            {
                Success = false,
                Message = $"You've been banned from {configuration.ServerName}"
            });

        var result = await _signInManager.CheckPasswordSignInAsync(user, form.Password, true);

        if (!result.Succeeded)
            return Unauthorized(new ErrorResponse
            {
                Success = false,
                Message = result.Convert()
            });

        var token = _jwtAuthentication.GetToken(user);

        return Ok(new LoginResponse
        {
            Success = true,
            Token = token
        });
    }


    [Route("/auth/game-login")]
    [HttpPost]
    public async Task<IActionResult> GameAuthenticateAsync([FromBody] GameLoginForm form,
        CancellationToken cancellationToken)
    {
        var (user, identity) = await _identityService.GetPrimaryIdentityByLicense(form.License,  cancellationToken: cancellationToken);
        if (user is null) return Utils.Responses.UserNotFoundInCad();
        if (identity is null) return Utils.Responses.NoPrimaryIdentity();

        if (await _userManager.IsInRoleAsync(user, RoleValues.Banned))
        {
            var banRecord = user.BanRecords.MaxBy(x => x.DateCreated);
            var banResponse = _mapper.Map<BanResponse>(banRecord);

            return Unauthorized(banResponse);
        }

        var vehicles = await _vehicleService.GetVehiclesAsync(user, identity.Id, false, cancellationToken);

        var login = await _gameLoginService.CreateLoginAsync(user, form.Name, cancellationToken);

        return Ok(new
        {
            success = true,
            vehicles,
            identity,
            session_id = login.Id,
            profile = new UserProfile
            {
                UserName = user.UserName,
                Success = true,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray()
            }
        });
    }
}