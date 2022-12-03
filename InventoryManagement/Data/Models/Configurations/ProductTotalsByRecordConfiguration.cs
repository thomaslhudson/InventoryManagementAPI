using InventoryManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Data.Models.Configurations
{
    public class ProductTotalsByRecordConfiguration : IEntityTypeConfiguration<ProductTotalsByRecord>
    {
        public void Configure(EntityTypeBuilder<ProductTotalsByRecord> builder)
        {
            builder.HasNoKey();
            builder.ToTable("ProductTotalsByRecord", t => t.ExcludeFromMigrations());
        }
    }
}
