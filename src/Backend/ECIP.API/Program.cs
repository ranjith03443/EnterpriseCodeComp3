using Serilog;
using ECIP.API.Extensions;
using ECIP.API.Middlewares;
using ECIP.Core.Interfaces;
using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Infrastructure.Extensions;
using ECIP.Infrastructure.Persistence;
using ECIP.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ECIP.API")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Enterprise Code Intelligence Platform API",
        Version = "v1.0",
        Description = "API for code analysis, architecture exploration, and AI-powered code comprehension"
    });
});

builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
builder.Services.AddScoped<IFileDiscoveryService, FileDiscoveryService>();
builder.Services.AddScoped<ILanguageDetector, LanguageDetector>();
builder.Services.AddScoped<IRepositoryScanner, RepositoryScannerService>();
builder.Services.AddScoped<IMetadataExtractionService, MetadataExtractionService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECIP API v1.0");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcipDbContext>();
    dbContext.Database.EnsureCreated();
}

// Log application startup
Log.Information("ECIP.API starting...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}