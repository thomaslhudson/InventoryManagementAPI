using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Data.Models.Configurations
{
    public class RecordItemConfiguration : IEntityTypeConfiguration<RecordItem>
    {
        public void Configure(EntityTypeBuilder<RecordItem> builder)
        {
            builder.Property(g => g.Id).HasDefaultValueSql("newsequentialid()");
            builder.HasKey(re => re.Id);

            builder.Property(re => re.Quantity).HasColumnType("decimal(9,3)");

            builder.HasOne(re => re.Record).WithMany(re => re.RecordItems).HasForeignKey(re => re.RecordId);

            builder.HasOne(re => re.Product).WithMany(re => re.RecordItems).HasForeignKey(re => re.ProductId);
            
            builder.HasIndex(re => new { re.RecordId, re.ProductId }).IsUnique();

            builder.ToTable("RecordItem");
        }
    }
}