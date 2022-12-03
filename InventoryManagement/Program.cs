using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using Serilog;
using InventoryManagement.Helpers.JSON;
using InventoryManagement.Filters;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Mvc.Formatters;
using InventoryManagement.Data.DataTransferObjects;
using Microsoft.AspNetCore.Hosting;
using InventoryManagement.Data.Migrations.MigrationManager;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.File("./logs/im-api-log-startup.txt")
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, lc) => lc.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddControllers(options => 
        { 
            options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
        })
        .AddJsonOptions(options => 
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    var connectionString = builder.Configuration.GetConnectionString("Default");
    builder.Services.AddDbContextPool<InventoryContext>
        (options => options.UseSqlServer(connectionString).UseExceptionProcessor());

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    #region Comments
    // '.UseExceptionProcessor' takes advantage of EntityFramework.Exceptions
    // and allows for catching more detailed database exceptions
    // (e.g. Unique Constraint Exception - see ExceptionHandler.cs for full list)
    #endregion
    builder.Services.AddScoped<ExceptionHandler>();
    builder.Services.AddTransient<ProductDto>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();

    app.MigrateDatabase();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}