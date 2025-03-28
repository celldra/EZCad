using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class TransactionService : ITransactionService
{
    private readonly EzCadDataContext _dataContext;
    private readonly IIdentityService _identityService;

    public TransactionService(EzCadDataContext dataContext, IIdentityService identityService)
    {
        _dataContext = dataContext;
        _identityService = identityService;
    }

    public async Task<BaseResponse> MakeTransactionAsync(double amount, string reason,
        User? targetUser = null, User? sourceUser = null, Identity? targetIdentity = null,
        Identity? sourceIdentity = null, bool isFine = false, CancellationToken cancellationToken = default)
    {
        // Money check
        if (sourceIdentity is not null && !isFine && sourceIdentity?.Money < amount)
            return new ErrorResponse
            {
                Success = false,
                Message = "Not enough funds"
            };

        // Add and remove the money from both users (if the from is null)

        Transaction? sourceTransaction = null;
        Transaction? targetTransaction = null;

        if (sourceIdentity is not null && sourceUser is not null)
        {
            sourceIdentity.Money -= amount;
            await _identityService.UpdateIdentityAsync(sourceUser, sourceIdentity.Id, sourceIdentity,
                cancellationToken);

            // Create the transaction log
            sourceTransaction = new Transaction
            {
                Description = reason,
                Amount = amount,
                Increase = false,
                FromIdentity = sourceIdentity,
                ToIdentity = targetIdentity
            };
            await CreateTransactionAsync(sourceUser, sourceIdentity, sourceTransaction, cancellationToken);
        }

        if (targetIdentity is not null && targetUser is not null)
        {
            targetIdentity.Money += amount;
            await _identityService.UpdateIdentityAsync(targetUser, targetIdentity.Id, targetIdentity,
                cancellationToken);

            // Create the transaction log
            targetTransaction = new Transaction
            {
                Description = reason,
                Amount = amount,
                Increase = true,
                FromIdentity = sourceIdentity,
                ToIdentity = targetIdentity
            };
            await CreateTransactionAsync(targetUser, targetIdentity, targetTransaction, cancellationToken);
        }

        return new TransactionResponse
        {
            Success = true,
            Message = "Transaction completed",
            TargetBalance = targetIdentity?.Money ?? 0f,
            Balance = sourceIdentity?.Money ?? 0f,
            TransactionId = sourceTransaction?.Id ?? string.Empty,
            TargetTransactionId = targetTransaction?.Id ?? string.Empty
        };
    }

    public async Task<Transaction?> GetTransactionAsync(User user, Identity identity, string id,
        CancellationToken cancellationToken = default)
    {
        return
            await _dataContext.Transactions.SingleOrDefaultAsync(x => x.Id == id && x.HostIdentity.Id == identity.Id,
                cancellationToken);
    }

    public async Task<Transaction?> CreateTransactionAsync(User user, Identity identity, Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        transaction.HostIdentity = identity;

        await _dataContext.AddAsync(transaction, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public async Task<List<Transaction>> GetTransactionsAsync(User user, Identity identity,
        CancellationToken cancellationToken = default)
    {
        return await _dataContext.Transactions.Where(x => x.HostIdentity.Id == identity.Id)
            .ToListAsync(cancellationToken);
    }
}