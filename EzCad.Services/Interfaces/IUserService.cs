using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetUserByDiscordIdAsync(string discordId, CancellationToken cancellationToken = default);
}