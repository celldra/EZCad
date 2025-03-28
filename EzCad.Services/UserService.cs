using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class UserService : IUserService
{
    private readonly EzCadDataContext _dataContext;

    public UserService(EzCadDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserByDiscordIdAsync(string discordId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Users.SingleOrDefaultAsync(x => x.DiscordId == discordId, cancellationToken);
    }
}