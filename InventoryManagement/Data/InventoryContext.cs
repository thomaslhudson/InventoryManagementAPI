using InventoryManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InventoryManagement.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
        {
        }

        // OnConfiguring can't be used because 'AddDbContextPool' is used instead of 'AddDbContext' in Program.cs
        // 'UseExceptionProcessor' has been moved to the 'AddDbContextPool' call in Program.cs
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseExceptionProcessor();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Record> Record { get; set; }

        public DbSet<RecordItem> RecordItem { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Group> Group { get; set; }

        public DbSet<GroupTotalsByRecord> GroupSubTotalsByRecord { get; set; }

        public DbSet<ProductTotalsByRecord> ProductTotalsByRecord { get; set; }
    }
}