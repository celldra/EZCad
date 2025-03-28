using EzCad.Services.Interfaces;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers.Administrative;

[ApiController]
[Route("/admin/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize(Roles = RoleValues.Administrator)]
public class AnalyticsController : ControllerBase
{
    private readonly IGameLoginService _gameLoginService;

    public AnalyticsController(IGameLoginService gameLoginService)
    {
        _gameLoginService = gameLoginService;
    }

    [Route("/admin/analytics/logins")]
    [HttpGet]
    public async Task<IActionResult> GetLoginAnalyticsAsync(CancellationToken cancellationToken)
    {
        var logins = await _gameLoginService.GetLoginsAsync(cancellationToken);
        // Trim down to only today's logins

        var todayLogins = logins.Where(x => x.DateCreated.Date == DateTime.UtcNow.Date);

        return Ok(todayLogins);
    }
}