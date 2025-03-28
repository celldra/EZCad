using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers.Administrative;

[ApiController]
[Route("/admin/identities/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize(Roles = RoleValues.Administrator)]

public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;

    public IdentityController(UserManager<User> userManager, IIdentityService identityService)
    {
        _userManager = userManager;
        _identityService = identityService;
    }

    [Route("/admin/{userId:required}/identities/{identityId:required}")]
    [HttpPut]
    public async Task<IActionResult> UpdateBalanceAsync(string userId, string identityId, [FromBody] BalanceForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();
        
        var targetUser = await _userManager.FindByIdAsync(userId);

        var identity = await _identityService.GetIdentityAsync(targetUser, identityId, true, cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        identity.Money = form.Balance;

        await _identityService.UpdateIdentityAsync(targetUser, identityId, identity, cancellationToken);

        return NoContent();
    }
}