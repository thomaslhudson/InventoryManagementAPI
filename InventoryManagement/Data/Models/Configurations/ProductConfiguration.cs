using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InventoryManagement.Data.Models.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(g => g.Id).HasDefaultValueSql("newsequentialid()");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.HasIndex(p => p.Name).IsUnique().HasDatabaseName("UX_Prouct_Name");

            builder.Property(p => p.Upc).HasMaxLength(12).IsFixedLength(true); //char(12)
            builder.Property(p => p.Upc).IsRequired(false); //Allow the Upc to be null
            builder.HasIndex(p => p.Upc).HasFilter("Upc IS NOT NULL") // Index only non-null records
                .IsUnique(true)
                .HasDatabaseName("UX_Product_Upc_Exclude_Nulls");

            builder.Property(p => p.UnitPrice).HasColumnType("decimal(9,3)");

            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(false).ValueGeneratedNever();

            builder.HasOne(p => p.Group).WithMany(p => p.Products);

            builder.ToTable("Product");
        }
    }
}