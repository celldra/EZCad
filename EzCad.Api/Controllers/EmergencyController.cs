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
[Route("/emergency/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class EmergencyController : ControllerBase
{
    private readonly IEmergencyService _emergencyService;
    private readonly IIdentityService _identityService;
    private readonly UserManager<User> _userManager;

    public EmergencyController(IEmergencyService emergencyService, IIdentityService identityService,
        UserManager<User> userManager)
    {
        _emergencyService = emergencyService;
        _identityService = identityService;
        _userManager = userManager;
    }

    [Route("/emergency/reports")]
    [HttpGet]
    [Authorize(Roles = $"{RoleValues.Police},{RoleValues.Medical}")]
    public async Task<IActionResult> GetReportsAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var reports = await _emergencyService.GetAllReports(cancellationToken);

        return Ok(reports);
    }

    [Route("/emergency/reports")]
    [HttpPost]
    public async Task<IActionResult> CreateEmergencyReportAsync([FromBody] EmergencyReportForm form,
        CancellationToken cancellationToken)
    {
        var (user, identity) =
            await _identityService.GetPrimaryIdentityByLicense(form.ReporterLicenseId, true, cancellationToken);

        if (user is null || identity is null)
            return Utils.Responses.CadNotLinked();

        var report = await _emergencyService.CreateReport(form.ReporterLicenseId, form.Description, form.Area,
            form.PostCode, cancellationToken);

        return StatusCode(201, new CreatedResponse<EmergencyReport>(report));
    }
}