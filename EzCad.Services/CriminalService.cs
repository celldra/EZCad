using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class CriminalService : ICriminalService
{
    private readonly EzCadDataContext _dataContext;

    public CriminalService(EzCadDataContext dataContext)
    {
        _dataContext = dataContext;
    }


    public async Task<CriminalRecord?> GetRecordAsync(Identity identity, string id,
        CancellationToken cancellationToken = default)
    {
        return await _dataContext.CriminalRecords.SingleOrDefaultAsync(x => x.Offender.Id == identity.Id,
            cancellationToken);
    }

    public async Task<CriminalRecord?> CreateRecordAsync(Identity identity, CriminalRecord record,
        CancellationToken cancellationToken = default)
    {
        record.Officer = identity;

        await _dataContext.AddAsync(record, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return record;
    }

    public async Task UpdateRecordAsync(Identity identity, string recordId, CriminalRecord newRecord,
        CancellationToken cancellationToken = default)
    {
        var record = await GetRecordAsync(identity, recordId, cancellationToken);
        if (record is null) return;

        record.Action = newRecord.Action;
        record.Offence = newRecord.Offence;
        record.Officer = newRecord.Officer;
        record.Offender = newRecord.Offender;

        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CriminalRecord>> GetRecordsAsync(Identity identity,
        CancellationToken cancellationToken = default)
    {
        return await _dataContext.CriminalRecords.Where(x => x.Offender.Id == identity.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CriminalRecord>> GetRecordsAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.CriminalRecords.ToListAsync(cancellationToken);
    }

    public async Task DeleteRecordAsync(Identity identity, string recordId,
        CancellationToken cancellationToken = default)
    {
        var record = await GetRecordAsync(identity, recordId, cancellationToken);
        if (record is null) return;

        _dataContext.Remove(record);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }
}