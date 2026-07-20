using System.Text.Json.Serialization;
using CynoHub.Api.Middleware;
using CynoHub.Api.Services;
using CynoHub.Api.Swagger;
using CynoHub.Application;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<RequireBreederHeaderFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register application & infrastructure layers
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBreederService, BreederService>();
builder.Services.AddApplication();
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=cynohub.db";
builder.Services.AddInfrastructure(opt => opt.UseSqlite(connectionString));

var app = builder.Build();

// Seed database on startup in development
if (app.Environment.IsDevelopment())
{
    await app.Services.SeedDatabaseAsync();
    await app.Services.EnsureSqsQueueCreatedAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling must be first
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<BreederAuthenticationMiddleware>();

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

app.Run();
public partial class Program { }
