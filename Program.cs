using Microsoft.EntityFrameworkCore;
using ReactCRUDAPI.Data;
using NLog.Web;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure NLog 
var logger = NLogBuilder.ConfigureNLog("D:\\React JS\\ReactCRUDAPI\\nlog.config.xml").GetCurrentClassLogger();

try
{
   

    logger.Debug("Starting application setup...");

    builder.Services.AddControllers();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", builder =>
            builder.WithOrigins("http://localhost:3000", "https://localhost:44326")
                   .AllowAnyMethod()
                   .AllowAnyHeader());
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Logging.AddNLog();  // Add NLog as the logging provider
    var app = builder.Build();

    // Middleware pipeline configuration
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowReactApp");
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application failed to start.");
    throw;
}
finally
{
    //flush and close the NLog resources
    NLog.LogManager.Shutdown();
}