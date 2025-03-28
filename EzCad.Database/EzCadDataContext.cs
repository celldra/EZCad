using EzCad.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Database;

public class EzCadDataContext : IdentityDbContext<User>
{
    public EzCadDataContext(DbContextOptions<EzCadDataContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    public DbSet<Identity> Identities { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CriminalRecord> CriminalRecords { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<EmergencyReport> EmergencyReports { get; set; }
    public DbSet<Configuration> Configurations { get; set; }
}