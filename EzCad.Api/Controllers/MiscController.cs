using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class MiscController : ControllerBase
{
    private readonly EzCadDataContext _dataContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public MiscController(EzCadDataContext dataContext, RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        _dataContext = dataContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [Route("/migrate-db")]
    [HttpGet]
    public async Task<IActionResult> MigrateDbAsync(CancellationToken cancellationToken)
    {
        await _dataContext.Database.MigrateAsync(cancellationToken);

        return NoContent();
    }

    [Route("/setup-roles")]
    [HttpGet]
    public async Task<IActionResult> SetupRolesAsync(CancellationToken cancellationToken)
    {
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.Administrator});
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.Moderator});
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.Medical});
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.Police});
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.ArmedPolice});
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.Banned});
        await _roleManager.CreateAsync(new IdentityRole {Name = RoleValues.Privileged});

        var user = await _userManager.FindByNameAsync("harry");

        await _userManager.AddToRolesAsync(user, new[]
        {
            RoleValues.Medical,
            RoleValues.Police,
            RoleValues.Administrator,
            RoleValues.Moderator,
            RoleValues.Privileged,
            RoleValues.ArmedPolice
        });

        return NoContent();
    }
}