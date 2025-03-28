using AutoMapper;
using EzCad.Api.Responses;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Models;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers.Administrative;

[ApiController]
[Route("/admin/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize(Roles = RoleValues.Administrator)]
public class UserController : ControllerBase
{
    private readonly IGameLoginService _gameLoginService;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;
    private readonly IVehicleService _vehicleService;

    public UserController(IMapper mapper, IUserService userService, IIdentityService identityService,
        UserManager<User> userManager, IGameLoginService gameLoginService, IVehicleService vehicleService)
    {
        _mapper = mapper;
        _userService = userService;
        _identityService = identityService;
        _userManager = userManager;
        _gameLoginService = gameLoginService;
        _vehicleService = vehicleService;
    }

    [Route("/admin/logins/{searchQuery:required}")]
    [HttpGet]
    public async Task<IActionResult> SearchLoginsAsync(string searchQuery, CancellationToken cancellationToken)
    {
        var logins = await _gameLoginService.GetLoginsAsync(cancellationToken);
        // Trim down to only today's logins

        var todayLogins = logins.Where(x => x.DateCreated.Date == DateTime.UtcNow.Date);
        var searchedLogins = todayLogins.YieldFuzzySearchResults(searchQuery);

        return Ok(searchedLogins);
    }

    [Route("/admin/users")]
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersAsync(cancellationToken);

        var mappedUsers = users.Select(x => _mapper.Map<UserProfile>(x));

        return Ok(mappedUsers);
    }

    [Route("/admin/users/{searchQuery:required}")]
    [HttpGet]
    public async Task<IActionResult> SearchUsersAsync(string searchQuery, CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersAsync(cancellationToken);

        var mappedUsers = users.Select(x => _mapper.Map<UserProfile>(x));

        var searchResults = mappedUsers.YieldFuzzySearchResults(searchQuery);

        return Ok(searchResults);
    }

    [Route("/admin/user/{id:required}")]
    [HttpGet]
    public async Task<IActionResult> GetUserAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        var mappedUser = _mapper.Map<UserProfile>(user);

        mappedUser.Roles = (await _userManager.GetRolesAsync(user)).ToArray();

        return Ok(mappedUser);
    }

    [Route("/admin/user/{id:required}/identities")]
    [HttpGet]
    public async Task<IActionResult> GetUserIdentitiesAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        var identities = await _identityService.GetIdentitiesAsync(user, cancellationToken: cancellationToken);

        return Ok(identities);
    }

    [Route("/admin/user/{id:required}/vehicles")]
    [HttpGet]
    public async Task<IActionResult> GetUserVehiclesAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        var identities = await _identityService.GetIdentitiesAsync(user, cancellationToken: cancellationToken);
        var vehicles = new List<Vehicle>();
        foreach (var identity in identities)
            vehicles.AddRange((await _vehicleService.GetVehiclesAsync(user, identity.Id, false, cancellationToken))!);


        return Ok(vehicles);
    }

    [Route("/admin/user/{id:required}/logins")]
    [HttpGet]
    public async Task<IActionResult> GetUserLoginsAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        var logins = await _gameLoginService.GetLoginsAsync(user, cancellationToken);

        return Ok(logins);
    }

    [Route("/admin/user/{id:required}/give-role/{roleName:required}")]
    [HttpPut]
    public async Task<IActionResult> GiveUserRoleAsync(string id, string roleName, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        await _userManager.AddToRoleAsync(user, roleName);

        return NoContent();
    }

    [Route("/admin/user/{id:required}/remove-role/{roleName:required}")]
    [HttpDelete]
    public async Task<IActionResult> RemoveUserRoleAsync(string id, string roleName,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        await _userManager.RemoveFromRoleAsync(user, roleName);

        return NoContent();
    }

    [Route("/admin/user/{id:required}/ban")]
    [HttpPost]
    public async Task<IActionResult> BanUserAsync([FromBody] BanUserForm form, string id,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        var targetUser = await _userManager.FindByIdAsync(id);

        var record = await _gameLoginService.BanUserAsync(user, targetUser, form.Reason, form.IsPermanent,
            form.Expiration, cancellationToken);

        return StatusCode(201, new CreatedResponse<BanRecord>(record));
    }

    [Route("/admin/user/{id:required}/delete")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUserAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return Utils.Responses.UserNotFound();

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = result.Convert()
            });

        return NoContent();
    }
}