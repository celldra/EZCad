using AutoMapper;
using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Shared.Models;
using EzCad.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("/user/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class UserController : ControllerBase
{
    private readonly EzCadDataContext _dataContext;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserController(UserManager<User> userManager, EzCadDataContext dataContext, IMapper mapper)
    {
        _userManager = userManager;
        _dataContext = dataContext;
        _mapper = mapper;
    }

    [Route("/user/{id:required}/link/{licenseId:required}")]
    [HttpGet]
    public async Task<IActionResult> LinkUserAsync(string id, string licenseId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id);

        // Trim out the license part
        licenseId = licenseId.Replace("license:", string.Empty);

        if (await _dataContext.Users.AnyAsync(x => x.LicenseId == licenseId, cancellationToken))
            return Ok(new ErrorResponse
            {
                Success = false,
                Message = "Cannot link your account"
            });

        user.IsLinked = true;
        user.LicenseId = licenseId;

        await _userManager.UpdateAsync(user);

        return Ok(new ErrorResponse
        {
            Success = true,
            Message = "FiveM account linked successfully"
        });
    }

    [Route("/user")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserProfileAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var profile = _mapper.Map<UserProfile>(user);
        profile.Roles = (await _userManager.GetRolesAsync(user)).ToArray();

        return Ok(profile);
    }

    [Route("/user/{licenseId:required}")]
    [HttpGet]
    public async Task<IActionResult> GetUserDetailsFromLicenseAsync(string licenseId,
        CancellationToken cancellationToken)
    {
        licenseId = licenseId.Replace("license:", string.Empty);
        var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.LicenseId == licenseId, cancellationToken);
        if (user is null) return Utils.Responses.UserNotFoundInCad();

        var identity =
            await _dataContext.Identities.SingleOrDefaultAsync(x => x.HostUser.Id == user.Id && x.IsPrimary,
                cancellationToken);
        if (identity is null) return Utils.Responses.NoPrimaryIdentity();

        var vehicles = await _dataContext.Vehicles.Where(x => x.HostIdentity.Id == identity.Id)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            success = true,
            vehicles,
            identity
        });
    }
}