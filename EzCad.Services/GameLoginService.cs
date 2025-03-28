using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class GameLoginService : IGameLoginService
{
    private readonly EzCadDataContext _dataContext;
    private readonly UserManager<User> _userManager;

    public GameLoginService(EzCadDataContext dataContext, UserManager<User> userManager)
    {
        _dataContext = dataContext;
        _userManager = userManager;
    }

    public async Task<Login> CreateLoginAsync(User user, string name, CancellationToken cancellationToken = default)
    {
        var l = new Login
        {
            Name = name,
            HostUser = user
        };

        await _dataContext.AddAsync(l, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return l;
    }

    public async Task<List<Login>> GetLoginsAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Logins.ToListAsync(cancellationToken);
    }

    public async Task<List<Login>> GetLoginsAsync(User user, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Logins.Where(x => x.HostUser.Id == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<BanRecord> BanUserAsync(User user, User targetUser, string reason, bool isPermanent,
        DateTime expiration, CancellationToken cancellationToken = default)
    {
        var r = new BanRecord
        {
            BannedBy = user,
            Reason = reason,
            IsPermanent = isPermanent,
            Expiration = expiration
        };

        targetUser.BanRecords.Add(r);
        await _dataContext.AddAsync(r, cancellationToken);

        await _dataContext.SaveChangesAsync(cancellationToken);

        await _userManager.AddToRoleAsync(targetUser, RoleValues.Banned);

        return r;
    }
}