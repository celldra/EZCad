using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Services.Utils;
using EzCad.Shared.Forms;
using EzCad.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Controllers;

[ApiController]
[Route("/identities/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class TransactionController : ControllerBase
{
    private readonly EzCadDataContext _dataContext;
    private readonly IIdentityService _identityService;
    private readonly ITransactionService _transactionService;
    private readonly UserManager<User> _userManager;
    private readonly IJobService _jobService;

    public TransactionController(ITransactionService transactionService, IIdentityService identityService,
        UserManager<User> userManager, EzCadDataContext dataContext, IJobService jobService)
    {
        _transactionService = transactionService;
        _identityService = identityService;
        _userManager = userManager;
        _dataContext = dataContext;
        _jobService = jobService;
    }

    // This is a vMenu API, this will never be called by the CAD frontend
    [Route("/transactions/{licenseId:required}/send-money")]
    [HttpPost]
    public async Task<IActionResult> SendMoneyAsync(string licenseId, [FromBody] SendMoneyForm form,
        CancellationToken cancellationToken)
    {
        // This part gets both valid CAD users for the targeted things
        var user = await _dataContext.GetUserByLicenseAsync(licenseId, cancellationToken);
        if (user is null) return Utils.Responses.CadNotLinked();

        var targetUser = await _dataContext.GetUserByLicenseAsync(form.ToLicenseId, cancellationToken);
        if (targetUser is null) return Utils.Responses.UserNotFoundInCad();

        var targetPrimaryIdentity = await _identityService.GetPrimaryIdentityAsync(targetUser, true, cancellationToken);
        var primaryIdentity = await _identityService.GetPrimaryIdentityAsync(user, true, cancellationToken);

        if (targetPrimaryIdentity is null || primaryIdentity is null) return Utils.Responses.NoPrimaryIdentity();

        var response = await _transactionService.MakeTransactionAsync(form.Amount, form.Description, targetUser,
            user, targetPrimaryIdentity, primaryIdentity, false, cancellationToken);

        if (response.Success) return Ok(response);

        return BadRequest(response);
    }

    [Route("/transactions/{licenseId:required}/upgrade-rank/{rankId:int:required}")]
    [HttpGet]
    public async Task<IActionResult> UpgradeRankAsync(string licenseId, int rankId, CancellationToken cancellationToken)
    {
        // This part gets both valid CAD users for the targeted things
        var user = await _dataContext.GetUserByLicenseAsync(licenseId, cancellationToken);
        if (user is null) return Utils.Responses.CadNotLinked();

        var primaryIdentity = await _identityService.GetPrimaryIdentityAsync(user, true, cancellationToken);

        if (primaryIdentity is null) return Utils.Responses.NoPrimaryIdentity();

        // Convert into an amount
        var amount = rankId switch
        {
            1 => 100000,
            2 => 500000,
            _ => 0
        };

        if (amount == 0)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "Invalid rank specified"
            });

        // Setup role names and such
        var roleName = rankId switch
        {
            1 => "Rank1",
            2 => "Rank2",
            _ => throw new ArgumentOutOfRangeException(nameof(rankId), rankId,
                null) // This will never happen (hopefully)
        };

        // Only add when the transaction is known to be successful
        var response = await _transactionService.MakeTransactionAsync(amount, "Rank upgrade", null, user, null,
            primaryIdentity, false, cancellationToken);

        if (!response.Success) return BadRequest(response);

        await _userManager.AddToRoleAsync(user, roleName);

        return Ok(new ErrorResponse
        {
            Success = true,
            Message =
                "Your rank has been upgraded! ~g~You will need to exit the game and join back for the permission changes to take effect~s~"
        });
    }

    // This is a vMenu API, this will never be called by the CAD frontend
    [Route("/transactions/{licenseId:required}/collect-salary")]
    [HttpGet]
    public async Task<IActionResult> CollectSalaryAsync(string licenseId, CancellationToken cancellationToken)
    {
        // This part gets both valid CAD users for the targeted things
        var user = await _dataContext.GetUserByLicenseAsync(licenseId, cancellationToken);
        if (user is null) return Utils.Responses.CadNotLinked();

        var primaryIdentity = await _identityService.GetPrimaryIdentityAsync(user, true, cancellationToken);

        if (primaryIdentity is null) return Utils.Responses.NoPrimaryIdentity();

        if (user.LastBenefitCollection.AddMinutes(45) > DateTime.UtcNow)
            return BadRequest(new ErrorResponse
            {
                Success = false,
                Message = "You need to wait before collecting your salary again"
            });

        // Determine how much money they get from the salary
        var money = (double)1000;
        if (primaryIdentity.JobId is not null)
        {
            // Get the job they have and the salary
            var job = await _jobService.GetJobByIdAsync(primaryIdentity.JobId?.ToString() ?? string.Empty,
                cancellationToken);

            if (job is not null)
            {
                money = job.Salary;
            }
        }

        // This is temporary, we'll fix this up soon
        primaryIdentity.Money += money;
        user.LastBenefitCollection = DateTime.UtcNow;

        // Now update the entries in the DB
        await _userManager.UpdateAsync(user);
        await _identityService.UpdateIdentityAsync(user, primaryIdentity.Id, primaryIdentity, cancellationToken);

        // Create the transaction log
        var transaction = new Transaction
        {
            Description = "Salary received",
            Amount = money,
            Increase = true,
            FromIdentity = null,
            ToIdentity = primaryIdentity
        };

        await _transactionService.CreateTransactionAsync(user, primaryIdentity, transaction, cancellationToken);

        return Ok(new TransactionResponse
        {
            Success = true,
            Message = "Transaction has completed successfully!",
            TransactionId = transaction.Id,
            Balance = primaryIdentity.Money
        });
    }

    [Route("/identities/{identityId:required}/transactions/{transactionId:required}")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIdentityTransactionsAsync(string identityId, string transactionId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, cancellationToken: cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        var transaction =
            await _transactionService.GetTransactionAsync(user, identity, transactionId, cancellationToken);
        if (transaction is null) return Utils.Responses.TransactionNotFound();

        return Ok(transaction);
    }

    [Route("/identities/{identityId:required}/transactions")]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIdentityTransactionsAsync(string identityId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Utils.Responses.UserNotAuthorized();

        var identity = await _identityService.GetIdentityAsync(user, identityId, cancellationToken: cancellationToken);
        if (identity is null) return Utils.Responses.IdentityNotFound();

        var transactions = await _transactionService.GetTransactionsAsync(user, identity, cancellationToken);
        var sortedTransactions = transactions.OrderByDescending(x => x.DateCreated);

        return Ok(sortedTransactions);
    }
}