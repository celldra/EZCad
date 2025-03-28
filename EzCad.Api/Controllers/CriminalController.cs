using EzCad.Api.Responses;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Forms;
using EzCad.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("/identities/{identityId:required}/criminal-records/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]

public class CriminalController : ControllerBase
{
    private readonly ICriminalService _criminalService;
    private readonly IIdentityService _identityService;
    private readonly ITransactionService _transactionService;
    private readonly UserManager<User> _userManager;

    public CriminalController(ICriminalService criminalService, UserManager<User> userManager,
        IIdentityService identityService, ITransactionService transactionService)
    {
        _criminalService = criminalService;
        _userManager = userManager;
        _identityService = identityService;
        _transactionService = transactionService;
    }

    [Route("/identities/criminal-records/{searchQuery:required}")]
    [HttpGet]
    [Authorize(Roles = "Police")]
    public async Task<IActionResult> SearchCriminalRecordsAsync(string searchQuery, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Utils.Responses.UserNotAuthorized();

        var records = (await _criminalService.GetRecordsAsync(cancellationToken))
            .YieldFuzzySearchResults(searchQuery);

        return Ok(records);
    }

    [Route("/identities/criminal-records")]
    [HttpGet]
    [Authorize(Roles = "Police")]
    public async Task<IActionResult> GetAllCriminalRecordsAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Utils.Responses.UserNotAuthorized();

        var records = await _criminalService.GetRecordsAsync(cancellationToken);

        return Ok(records.OrderByDescending(x => x.DateCreated));
    }

    [Route("/identities/{identityId:required}/criminal-records")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCriminalRecordsAsync(string identityId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, cancellationToken: cancellationToken);
        if (identity is null)
            return Utils.Responses.IdentityNotFound();

        var records = await _criminalService.GetRecordsAsync(identity, cancellationToken);

        return Ok(records);
    }


    [Route("/identities/{licenseId:required}/fine/{targetLicenseId:required}")]
    [HttpPost]
    public async Task<IActionResult> FineAsync(string licenseId, string targetLicenseId, [FromBody] FineForm form,
        CancellationToken cancellationToken)
    {
        var (user, identity) =
            await _identityService.GetPrimaryIdentityByLicense(licenseId,true, cancellationToken);
        var (offenderUser, offenderIdentity) =
            await _identityService.GetPrimaryIdentityByLicense(targetLicenseId, true, cancellationToken);

        if (user is null || offenderUser is null)
            return Utils.Responses.CadNotLinked();

        if (!await _userManager.IsInRoleAsync(user, "Police"))
            return Utils.Responses.UserNotAuthorized();

        if (offenderIdentity is null || identity is null)
            return Utils.Responses.NoPrimaryIdentity();

        var r = new CriminalRecord
        {
            Action = "Fine",
            Offence = form.Description,
            Offender = offenderIdentity
        };

        await _criminalService.CreateRecordAsync(offenderIdentity, r, cancellationToken);

        var response = await _transactionService.MakeTransactionAsync(form.Amount, form.Description, null, offenderUser,
            null, offenderIdentity, true, cancellationToken);

        if (!response.Success) return BadRequest(response);

        return Ok(response);
    }

    [Route("/identities/{identityId:required}/criminal-records/{recordId:required}")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetRecordAsync(string identityId, string recordId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, cancellationToken: cancellationToken);
        if (identity is null)
            return Utils.Responses.IdentityNotFound();

        var record = await _criminalService.GetRecordAsync(identity, recordId, cancellationToken: cancellationToken);
        if (record is null)
            return Utils.Responses.RecordNotFound();

        return Ok(record);
    }

    [Route("/identities/criminal-records")]
    [HttpPost]
    public async Task<IActionResult> CreateRecordAsync([FromBody] CriminalRecordForm form,
        CancellationToken cancellationToken)
    {
        var (user, identity) =
            await _identityService.GetPrimaryIdentityByLicense(form.OfficerLicenseId, true, cancellationToken);
        var (offenderUser, offenderIdentity) =
            await _identityService.GetPrimaryIdentityByLicense(form.OffenderLicenseId, true, cancellationToken);

        if (user is null || offenderUser is null)
            return Utils.Responses.CadNotLinked();

        if (!await _userManager.IsInRoleAsync(user, "Police"))
            return Utils.Responses.UserNotAuthorized();

        if (offenderIdentity is null || identity is null)
            return Utils.Responses.NoPrimaryIdentity();

        var r = new CriminalRecord
        {
            Action = form.Action,
            Offence = form.Offence,
            Offender = offenderIdentity
        };

        var record = await _criminalService.CreateRecordAsync(identity, r, cancellationToken);

        return StatusCode(201, new CreatedResponse<CriminalRecord>(record));
    }

    [Route("/identities/{identityId:required}/criminal-records/{recordId:required}")]
    [HttpPut]
    [Authorize(Roles = "Police")]
    public async Task<IActionResult> UpdateRecordAsync(string identityId, string recordId,
        [FromBody] CriminalRecordForm form,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, true, cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        var record = await _criminalService.GetRecordAsync(identity, recordId, cancellationToken);
        if (record is null) return Utils.Responses.RecordNotFound();

        record.Action = form.Action;
        record.Offence = form.Offence;

        await _criminalService.UpdateRecordAsync(identity, recordId, record, cancellationToken);

        return NoContent();
    }
}