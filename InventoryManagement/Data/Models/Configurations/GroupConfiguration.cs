using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Data.Models.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Property(g => g.Id).HasDefaultValueSql("newsequentialid()");
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
            builder.HasIndex(g => g.Name).IsUnique();

            builder.HasMany(g => g.Products).WithOne(g => g.Group).HasForeignKey(g => g.GroupId);

            builder.ToTable("Group");
        }
    }
}
