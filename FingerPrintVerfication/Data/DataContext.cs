using FingerPrintVerfication.Entity;
using Microsoft.EntityFrameworkCore;

namespace FingerPrintVerfication.Data;

public class DataContext : DbContext
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<FingerPrint> FingerPrints => Set<FingerPrint>();
    public DbSet<FingerPrintAudit> FingerPrintAudits => Set<FingerPrintAudit>();

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Person>().HasMany(e => e.FingerPrints)
            .WithOne(e => e.Person)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(builder);
    }
}