using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers.Administrative;

[ApiController]
[Route("/admin/jobs/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize(Roles = RoleValues.Administrator)]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly UserManager<User> _userManager;
    private readonly IIdentityService _identityService;

    public JobController(IJobService jobService, UserManager<User> userManager, IIdentityService identityService)
    {
        _jobService = jobService;
        _userManager = userManager;
        _identityService = identityService;
    }

    [Route("/admin/jobs/{jobId:required}")]
    [HttpGet]
    public async Task<IActionResult> GetJobByIdAsync(string jobId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var j = await _jobService.GetJobByIdAsync(jobId, cancellationToken);
        if (j is null) return Utils.Responses.JobNotFound();

        return Ok(j);
    }

    [Route("/jobs")]
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetJobsAsync(CancellationToken cancellationToken)
    {
        var j = await _jobService.GetAllJobsAsync(cancellationToken);
        return Ok(j);
    }

    [Route("/admin/jobs/{jobId:required}")]
    [HttpPut]
    public async Task<IActionResult> UpdateJobAsync(string jobId, [FromBody] JobForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var j = await _jobService.GetJobByIdAsync(jobId, cancellationToken);
        if (j is null) return Utils.Responses.JobNotFound();

        j.Name = form.Name;
        j.Salary = double.Parse(form.Salary);
        j.IsPublic = form.IsPublic;

        await _jobService.UpdateJobAsync(jobId, j, cancellationToken);

        return NoContent();
    }

    [Route("/admin/jobs/{jobId:required}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteJobAsync(string jobId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var j = await _jobService.GetJobByIdAsync(jobId, cancellationToken);
        if (j is null) return Utils.Responses.JobNotFound();

        await _jobService.DeleteJobAsync(jobId, cancellationToken);

        return NoContent();
    }

    [Route("/admin/jobs/")]
    [HttpPost]
    public async Task<IActionResult> CreateJobAsync([FromBody] JobForm form, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var j = new Job
        {
            Name = form.Name,
            Salary = double.Parse(form.Salary),
            IsPublic = form.IsPublic
        };

        var jobDb = await _jobService.CreateJobAsync(j, cancellationToken);
        if (jobDb is null)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "A job with that name already exists"
            });

        return StatusCode(201, new CreatedResponse<Job>(jobDb));
    }

    [Route("/admin/{userId:required}/identities/{identityId:required}/job/{jobId:required}")]
    [HttpGet]
    public async Task<IActionResult> AddToJobAsync(string userId, string identityId, string jobId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var targetUser = await _userManager.FindByIdAsync(userId);

        var identity = await _identityService.GetIdentityAsync(targetUser, identityId, true, cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        var job = await _jobService.GetJobByIdAsync(jobId, cancellationToken);
        if (job is null) return Utils.Responses.JobNotFound();

        identity.JobId = job.Id;

        await _identityService.UpdateIdentityAsync(user, identityId, identity, cancellationToken);

        return NoContent();
    }
    
    [Route("/{licenseId:required}/job/{jobId:required}")]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> AddToJobAsync(string licenseId, string jobId,
        CancellationToken cancellationToken)
    {
        var (user, identity) = await _identityService.GetPrimaryIdentityByLicense(licenseId, cancellationToken: cancellationToken);
        if (user is null || identity is null) return Utils.Responses.CadNotLinked();

        var job = await _jobService.GetJobByIdAsync(jobId, cancellationToken);
        if (job is null || !job.IsPublic) return Utils.Responses.JobNotFound();

        identity.JobId = job.Id;

        await _identityService.UpdateIdentityAsync(user, identity.Id, identity, cancellationToken);

        return Ok(new ErrorResponse
        {
            Success = true
        });
    }
    
    [Route("/admin/{userId:required}/identities/{identityId:required}/job")]
    [HttpDelete]
    public async Task<IActionResult> RemoveFromJobAsync(string userId, string identityId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var targetUser = await _userManager.FindByIdAsync(userId);

        var identity = await _identityService.GetIdentityAsync(targetUser, identityId, true, cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        identity.JobId = null;

        await _identityService.UpdateIdentityAsync(user, identityId, identity, cancellationToken);

        return NoContent();
    }
}