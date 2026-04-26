using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.AppointmentId).IsUnique();
        builder.Property(b => b.ConsultationFee).HasColumnType("decimal(10,2)");
        builder.Property(b => b.MedicineCharges).HasColumnType("decimal(10,2)");
        builder.Property(b => b.PaymentStatus).IsRequired().HasMaxLength(20);
    }
}
