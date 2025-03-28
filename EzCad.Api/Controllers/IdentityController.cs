using EzCad.Api.Responses;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("/identities/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;
    private readonly IVehicleService _vehicleService;

    public IdentityController(IIdentityService identityService, UserManager<User> userManager,
        IVehicleService vehicleService)
    {
        _identityService = identityService;
        _userManager = userManager;
        _vehicleService = vehicleService;
    }

    [Route("/identities")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIdentitiesAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identities = await _identityService.GetIdentitiesAsync(user, cancellationToken: cancellationToken);

        return Ok(identities);
    }

    [Route("/identities/{id:required}")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIdentityAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, id, cancellationToken: cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        return Ok(identity);
    }

    [Route("/identities")]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateIdentityAsync([FromBody] IdentityForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var i = new Identity
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            DateOfBirth = form.DateOfBirth,
            BirthPlace = form.BirthPlace,
            Sex = form.Sex
        };

        var identity = await _identityService.CreateIdentityAsync(user, i, cancellationToken);

        return StatusCode(201, new CreatedResponse<Identity>(identity));
    }

    [Route("/identities/{identityId:required}")]
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteIdentityAsync(string identityId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, true, cancellationToken);
        if (identity is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Identity not found"
            });

        foreach (var vehicle in (await _vehicleService.GetVehiclesAsync(user, identityId, true, cancellationToken))!)
            await _vehicleService.DeleteVehicleAsync(user, vehicle.Id, cancellationToken);

        await _identityService.DeleteIdentityAsync(user, identity.Id, cancellationToken);

        return NoContent();
    }

    [Route("/identities/{identityId:required}")]
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateIdentityAsync(string identityId, [FromBody] IdentityForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, true, cancellationToken);
        if (identity is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Identity not found"
            });

        identity.IsPrimary = form.IsPrimary;
        identity.FirstName = form.FirstName;
        identity.LastName = form.LastName;
        identity.DateOfBirth = form.DateOfBirth;
        identity.BirthPlace = form.BirthPlace;

        await _identityService.UpdateIdentityAsync(user, identityId, identity, cancellationToken);

        return NoContent();
    }
}