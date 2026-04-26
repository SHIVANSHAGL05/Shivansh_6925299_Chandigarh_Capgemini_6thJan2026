using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DepartmentName).IsRequired().HasMaxLength(100);
        builder.HasIndex(d => d.DepartmentName).IsUnique();
        builder.Property(d => d.Description).HasMaxLength(255);
    }
}
