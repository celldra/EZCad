using AutoMapper;
using EzCad.Api.Responses;
using EzCad.Database.Entities;
using EzCad.Redis.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Api.Controllers.Administrative;

[ApiController]
[Route("/admin/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize(Roles = RoleValues.Administrator)]
public class RoleController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IRedisCachingService _redis;

    public RoleController(RoleManager<IdentityRole> roleManager, IMapper mapper, IRedisCachingService redis)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _redis = redis;
    }

    [Route("/admin/roles")]
    [HttpGet]
    public async Task<IActionResult> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await _redis.GetOrSetRecordAsync("roles",
            async () => await _roleManager.Roles.ToListAsync(cancellationToken), false, cancellationToken);

        if (roles == null)
            return StatusCode(500, new ErrorResponse
            {
                Success = false,
                Message = "Failed to retrieve user roles, check that your database configuration is correct"
            });
        
        var friendlyRoles = roles.Select(r => _mapper.Map<UserRole>(r));

        return Ok(friendlyRoles);

    }

    [Route("/admin/roles")]
    [HttpPost]
    public async Task<IActionResult> CreateRoleAsync([FromBody] AddRoleForm form, CancellationToken cancellationToken)
    {
        var roles = await _redis.GetOrSetRecordAsync("roles",
            async () => await _roleManager.Roles.ToListAsync(cancellationToken), false, cancellationToken);

        if (roles != null && roles.Any(x => x.Name == form.Role)) return Utils.Responses.RoleAlreadyExists();

        var identityRole = new IdentityRole
        {
            Name = form.Role
        };

        await _roleManager.CreateAsync(identityRole);

        // Add role to cache
        roles?.Add(identityRole);
        await _redis.UpdateRecordAsync("roles", roles, cancellationToken: cancellationToken);

        return StatusCode(201, new CreatedResponse<IdentityRole>(identityRole));
    }

    [Route("/admin/roles/{roleId:required}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRoleAsync(string roleId, CancellationToken cancellationToken)
    {
        var roles = await _redis.GetOrSetRecordAsync("roles",
            async () => await _roleManager.Roles.ToListAsync(cancellationToken), false, cancellationToken);

        if (roles is null)
        {
            return StatusCode(500, new ErrorResponse
            {
                Success = false,
                Message = "Failed to retrieve user roles, check that your database configuration is correct"
            });
        }
        
        if (roles.All(x => x.Id != roleId)) return Utils.Responses.RoleNotFound();
        var role = roles.FirstOrDefault(x => x.Id == roleId);

        if (RoleValues.GetAllDefaultRoles().Contains(role.Name)) return Utils.Responses.CannotModifyDefaultRole();

        await _roleManager.DeleteAsync(role);
        
        // Remove role from cache
        roles.RemoveAll(x => x.Id == roleId);
        await _redis.UpdateRecordAsync("roles", roles, cancellationToken: cancellationToken);

        return NoContent();
    }

    [Route("/admin/roles/{roleId:required}")]
    [HttpPut]
    public async Task<IActionResult> UpdateRoleAsync(string roleId, [FromBody] AddRoleForm form,
        CancellationToken cancellationToken)
    {
        var roles = await _redis.GetOrSetRecordAsync("roles",
            async () => await _roleManager.Roles.ToListAsync(cancellationToken), false, cancellationToken);

        if (roles is null)
        {
            return StatusCode(500, new ErrorResponse
            {
                Success = false,
                Message = "Failed to retrieve user roles, check that your database configuration is correct"
            });
        }
        
        if (roles.All(x => x.Id != roleId)) return Utils.Responses.RoleNotFound();
        var role = roles.FirstOrDefault(x => x.Id == roleId);

        if (RoleValues.GetAllDefaultRoles().Contains(role!.Name)) return Utils.Responses.CannotModifyDefaultRole();

        role.Name = form.Role;

        await _roleManager.UpdateAsync(role);
        
        // Update cache
        roles.SingleOrDefault(x => x.Id == roleId)!.Name = form.Role;
        await _redis.UpdateRecordAsync("roles", roles, cancellationToken: cancellationToken);

        return NoContent();
    }
}