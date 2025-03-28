using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class JobService : IJobService
{
    private readonly EzCadDataContext _dataContext;

    public JobService(EzCadDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Job?> GetJobByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dataContext.FindAsync<Job>(new object[]{id}, cancellationToken: cancellationToken);
    }
    
    public async Task<Job?> GetJobByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Jobs.SingleOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<List<Job>?> GetAllJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _dataContext.Jobs.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Job?> CreateJobAsync(Job job, CancellationToken cancellationToken = default)
    {
        if (await GetJobByNameAsync(job.Name, cancellationToken) is not null) return null;
        
        await _dataContext.AddAsync(job, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);
        
        return job;
    }
    
    public async Task UpdateJobAsync(string jobId, Job newJob,
        CancellationToken cancellationToken = default)
    {
        var job = await GetJobByIdAsync(jobId, cancellationToken);
        if (job is null) return;
        
        job.IsPublic = newJob.IsPublic;
        job.Name = newJob.Name;
        job.Salary = newJob.Salary;

        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var job = await GetJobByIdAsync(jobId, cancellationToken);
        if (job is null) return;

        _dataContext.Remove(job);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }
}