using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class EmergencyService : IEmergencyService
{
    private readonly EzCadDataContext _dataContext;
    private readonly IIdentityService _identityService;

    public EmergencyService(EzCadDataContext dataContext, IIdentityService identityService)
    {
        _dataContext = dataContext;
        _identityService = identityService;
    }

    public async Task<List<EmergencyReport>> GetAllReports(CancellationToken cancellationToken = default)
    {
        return await _dataContext.EmergencyReports.ToListAsync(cancellationToken);
    }

    public async Task<EmergencyReport?> CreateReport(string licenseId, string description, string area,
        string postalCode,
        CancellationToken cancellationToken = default)
    {
        var (_, identity) = await _identityService.GetPrimaryIdentityByLicense(licenseId, true, cancellationToken);
        if (identity is null) return null;

        var r = new EmergencyReport
        {
            PostCode = postalCode,
            Description = description,
            Area = area,
            ReportingIdentity = identity
        };

        await _dataContext.AddAsync(r, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return r;
    }
}