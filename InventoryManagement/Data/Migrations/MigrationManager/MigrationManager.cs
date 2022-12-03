using InventoryManagement.Filters;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data.Migrations.MigrationManager
{
    public static class MigrationManager
    {
        public static WebApplication MigrateDatabase(this WebApplication webApp)
        {
            using IServiceScope scope = webApp.Services.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<InventoryContext>();
            try
            {
                appContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("MigrateDatabase failed: ", ex);
            }

            return webApp;
        }
    }
}
