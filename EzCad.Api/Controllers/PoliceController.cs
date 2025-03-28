using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("/admin/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize(Roles = RoleValues.Police)]
public class PoliceController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IIdentityService _identityService;
    private readonly IVehicleService _vehicleService;
    private readonly ICriminalService _criminalService;

    public PoliceController(UserManager<User> userManager, IVehicleService vehicleService,
        IIdentityService identityService, ICriminalService criminalService)
    {
        _userManager = userManager;
        _vehicleService = vehicleService;
        _identityService = identityService;
        _criminalService = criminalService;
    }

    [Route("/identities/police/{id:required}")]
    [HttpGet]
    [Authorize(Roles = $"{RoleValues.Police},{RoleValues.Administrator}")]
    public async Task<IActionResult> GetIdentityAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(id, cancellationToken: cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        return Ok(identity);
    }

    [Route("/identities/police/{identityId:required}/criminal-records")]
    [HttpGet]
    [Authorize(Roles = $"{RoleValues.Police},{RoleValues.Administrator}")]
    public async Task<IActionResult> GetCriminalRecordsAsync(string identityId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(identityId, cancellationToken: cancellationToken);
        if (identity is null)
            return Utils.Responses.IdentityNotFound();

        var records = await _criminalService.GetRecordsAsync(identity, cancellationToken);

        return Ok(records);
    }

    [Route("/identities/police/{id:required}/vehicles")]
    [HttpGet]
    [Authorize(Roles = $"{RoleValues.Police},{RoleValues.Administrator}")]
    public async Task<IActionResult> GetIdentityVehiclesAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(id, cancellationToken: cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        var vehicles = await _vehicleService.GetVehiclesAsync(identity.Id, false, cancellationToken);

        return Ok(vehicles);
    }
}