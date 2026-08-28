using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(200);

        // 1. Shadow Audit Property (exists in DB without polluting DTOs)
        builder.Property<DateTime>("LastUpdated")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // 2. Concurrency token mapped to PostgreSQL row version
        builder.Property(s => s.Version)
            .IsRowVersion();

        // 3. Soft-delete global query filter
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}