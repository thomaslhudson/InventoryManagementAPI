using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Data.Models.Configurations
{
    public class GroupTotalsByRecordConfiguration : IEntityTypeConfiguration<GroupTotalsByRecord>
    {
        public void Configure(EntityTypeBuilder<GroupTotalsByRecord> builder)
        {
            builder.HasNoKey();
            builder.ToTable("GroupTotalsByRecord", t => t.ExcludeFromMigrations());
        }
    }
}
