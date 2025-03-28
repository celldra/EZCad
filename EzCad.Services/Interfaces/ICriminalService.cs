using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface ICriminalService
{
    Task<CriminalRecord?> GetRecordAsync(Identity identity, string id, CancellationToken cancellationToken = default);

    Task<CriminalRecord?> CreateRecordAsync(Identity identity, CriminalRecord record,
        CancellationToken cancellationToken = default);

    Task UpdateRecordAsync(Identity identity, string recordId, CriminalRecord newRecord,
        CancellationToken cancellationToken = default);

    Task<List<CriminalRecord>> GetRecordsAsync(Identity identity, CancellationToken cancellationToken = default);
    Task<List<CriminalRecord>> GetRecordsAsync(CancellationToken cancellationToken = default);
}