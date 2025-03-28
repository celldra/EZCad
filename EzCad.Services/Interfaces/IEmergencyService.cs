using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IEmergencyService
{
    Task<List<EmergencyReport>> GetAllReports(CancellationToken cancellationToken = default);

    Task<EmergencyReport?> CreateReport(string licenseId, string description, string area, string postalCode,
        CancellationToken cancellationToken = default);
}