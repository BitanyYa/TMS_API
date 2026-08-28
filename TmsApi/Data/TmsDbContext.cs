using Microsoft.EntityFrameworkCore;
using TmsApi.Entities;

namespace TmsApi.Data;

public class TmsDbContext(DbContextOptions<TmsDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Automatically discovers and applies all IEntityTypeConfiguration classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TmsDbContext).Assembly);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Student>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Property("LastUpdated").CurrentValue = DateTime.UtcNow;
        }

    return base.SaveChangesAsync(cancellationToken);
    }
}