using CynoHub.Api.Middleware;
using CynoHub.Application;
using CynoHub.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application & infrastructure layers
builder.Services.AddApplication();
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=cynohub.db";
builder.Services.AddInfrastructure(opt => opt.UseSqlite(connectionString));

var app = builder.Build();

// Seed database on startup in development
if (app.Environment.IsDevelopment())
{
    await app.Services.SeedDatabaseAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling must be first
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
