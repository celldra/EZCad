using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IJobService
{
    Task<Job?> GetJobByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Job?> GetJobByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Job>?> GetAllJobsAsync(CancellationToken cancellationToken = default);
    Task<Job?> CreateJobAsync(Job job, CancellationToken cancellationToken = default);

    Task UpdateJobAsync(string jobId, Job newJob,
        CancellationToken cancellationToken = default);

    Task DeleteJobAsync(string jobId, CancellationToken cancellationToken = default);
}