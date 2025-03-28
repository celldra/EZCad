using EzCad.Api.Responses;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("/vehicles/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class VehicleController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService, IIdentityService identityService,
        UserManager<User> userManager)
    {
        _vehicleService = vehicleService;
        _identityService = identityService;
        _userManager = userManager;
    }

    [Route("/vehicles/{licenseId:required}/license-plate/{licensePlate:required}")]
    [HttpGet]
    public async Task<IActionResult> SearchByLicensePlateAsync(string licenseId, string licensePlate,
        CancellationToken cancellationToken)
    {
        var (user, identity) = await _identityService.GetPrimaryIdentityByLicense(licenseId, cancellationToken: cancellationToken);
        if (user is null || identity is null) return Utils.Responses.CadNotLinked();

        if (!await _userManager.IsInRoleAsync(user, RoleValues.Police)) return Utils.Responses.UserNotAuthorized();

        var vehicle = await _vehicleService.GetVehicleByLicenceAsync(licensePlate, false, cancellationToken);
        if (vehicle is null)
            return Ok(new ErrorResponse
            {
                Success = true,
                Message = "Vehicle is not registered!"
            });

        return Ok(vehicle);
    }

    [Route("/vehicles/{vehicleId:required}")]
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetVehicleAsync(string vehicleId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var vehicle = await _vehicleService.GetVehicleAsync(user, vehicleId, false, cancellationToken);
        if (vehicle is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Vehicle not found"
            });

        return Ok(vehicle);
    }
    
    [Route("/identities/{identityId:required}/vehicles")]
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetVehiclesAsync(string identityId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, cancellationToken: cancellationToken);
        var vehicles = await _vehicleService.GetVehiclesAsync(user, identityId, cancellationToken: cancellationToken);
        
        return Ok(vehicles);
    }

    [Route("/vehicles")]
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetVehiclesAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

            var identities = await _identityService.GetIdentitiesAsync(user, cancellationToken: cancellationToken);
        var vehicles = new List<Vehicle>();
        foreach (var identity in identities)
            vehicles.AddRange((await _vehicleService.GetVehiclesAsync(user, identity.Id, false, cancellationToken))!);

        return Ok(vehicles);
    }

    [Route("/vehicles")]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateVehicleAsync([FromBody] VehicleForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var v = new Vehicle
        {
            Manufacturer = form.Manufacturer,
            Model = form.Model,
            LicensePlate = form.LicensePlate,
            IsStolen = form.IsStolen,
            MotState = form.MotState,
            InsuranceState = form.InsuranceState
        };

        var identity = await _identityService.GetIdentityAsync(user, form.IdentityId, true, cancellationToken);
        if (identity is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Identity not found"
            });

        var createdVehicle = await _vehicleService.CreateVehicleAsync(identity, v, cancellationToken);

        return StatusCode(201, new CreatedResponse<Vehicle>(createdVehicle));
    }

    [Route("/vehicles/{id:required}")]
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteVehicleAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var vehicle = await _vehicleService.GetVehicleAsync(user, id, true, cancellationToken);
        if (vehicle is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Vehicle not found"
            });

        await _vehicleService.DeleteVehicleAsync(user, vehicle.Id, cancellationToken);

        return NoContent();
    }

    [Route("/vehicles/{id:required}")]
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateVehicleAsync(string id, [FromBody] VehicleForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var vehicle = await _vehicleService.GetVehicleAsync(user, id, true, cancellationToken);
        if (vehicle is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Vehicle not found"
            });

        var identity = await _identityService.GetIdentityAsync(user, form.IdentityId, true, cancellationToken);
        if (identity is null)
            return NotFound(new ErrorResponse
            {
                Success = false,
                Message = "Identity not found"
            });

        var newVehicle = new Vehicle
        {
            Manufacturer = form.Manufacturer,
            Model = form.Model,
            LicensePlate = form.LicensePlate,
            IsStolen = form.IsStolen,
            HostIdentity = identity,
            MotState = form.MotState,
            InsuranceState = form.InsuranceState
        };

        await _vehicleService.UpdateVehicleAsync(user, vehicle.Id, newVehicle, cancellationToken);

        return NoContent();
    }
}