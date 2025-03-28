using EzCad.Database.Entities;
using EzCad.Shared.Responses;

namespace EzCad.Services.Interfaces;

public interface ITransactionService
{
    Task<Transaction?> GetTransactionAsync(User user, Identity identity, string id,
        CancellationToken cancellationToken = default);

    Task<Transaction?> CreateTransactionAsync(User user, Identity identity, Transaction transaction,
        CancellationToken cancellationToken = default);

    Task<List<Transaction>> GetTransactionsAsync(User user, Identity identity,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends money from the source identity to the target identity
    /// </summary>
    /// <param name="amount">The amount to send</param>
    /// <param name="reason">The reason the money is being sent</param>
    /// <param name="sourceUser">The user object of the source</param>
    /// <param name="targetIdentity">The identity to send the money to</param>
    /// <param name="sourceIdentity">The source identity the money is coming from</param>
    /// <param name="isFine">
    ///     Whether the transaction is a fine, on which the money check will be fully ignored and money will
    ///     just be deducted
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <param name="targetUser">The user object of the target</param>
    /// <returns>A <see cref="ErrorResponse" /> on failure and a <see cref="TransactionResponse" /> on success</returns>
    Task<BaseResponse> MakeTransactionAsync(double amount, string reason,
        User? targetUser = null, User? sourceUser = null, Identity? targetIdentity = null,
        Identity? sourceIdentity = null, bool isFine = false, CancellationToken cancellationToken = default);
}