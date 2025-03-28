using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IGameLoginService
{
    Task<Login> CreateLoginAsync(User user, string name, CancellationToken cancellationToken = default);
    Task<List<Login>> GetLoginsAsync(CancellationToken cancellationToken = default);
    Task<List<Login>> GetLoginsAsync(User user, CancellationToken cancellationToken = default);

    Task<BanRecord> BanUserAsync(User user, User targetUser, string reason, bool isPermanent,
        DateTime expiration, CancellationToken cancellationToken = default);
}