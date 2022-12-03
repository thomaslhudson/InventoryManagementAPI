using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Data.Models.Configurations
{
    public class RecordConfiguration : IEntityTypeConfiguration<Record>
    {
        public void Configure(EntityTypeBuilder<Record> builder)
        {
            builder.Property(g => g.Id).HasDefaultValueSql("newsequentialid()");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Created)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasMany(r => r.RecordItems).WithOne(r => r.Record).HasForeignKey(r => r.RecordId);

            builder.HasIndex(r => new { r.Month, r.Year }).IsUnique();

            builder.ToTable("Record");
        }
    }
}