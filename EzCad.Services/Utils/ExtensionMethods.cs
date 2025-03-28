using EzCad.Database;
using EzCad.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services.Utils;

public static class ExtensionMethods
{
    public static async Task<User?> GetUserByLicenseAsync(this EzCadDataContext dataContext, string licenseId,
        CancellationToken cancellationToken = default)
    {
        licenseId = licenseId.Replace("license:", string.Empty);
        return await dataContext.Users.SingleOrDefaultAsync(x => x.LicenseId == licenseId, cancellationToken);
    }
}