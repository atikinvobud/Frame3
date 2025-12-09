using Backend.Models;
using BackEnd.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Backend.Repository;
using System.Text.Json;
using Microsoft.OpenApi.Models;
Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = false;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Frame3",
        Version = "v1"
    }));

var dbHost = builder.Configuration["DB_HOST"];
var dbPort = builder.Configuration["DB_PORT"];
var dbName = builder.Configuration["DB_NAME"];
var dbUser = builder.Configuration["DB_USER"];
var dbPassword = builder.Configuration["DB_PASSWORD"];
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.Configure<ApiUrls>(builder.Configuration.GetSection("Urls"));
builder.Services.Configure<FetchTimes>(builder.Configuration.GetSection("Time"));
builder.Services.AddDbContext<Context>(options => options.UseNpgsql(connectionString));
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IIssApiService,IssApiService>();
builder.Services.AddScoped<IIssRepository, IssRepository>();
builder.Services.AddScoped<ISpaceCacheRepository, SpaceCacheRepository>();
builder.Services.AddHostedService<IssBackgroundService>();
builder.Services.AddHostedService<ApodBackgroundService>();
builder.Services.AddHostedService<NeoBackgroundService>();
builder.Services.AddHostedService<DonkiFLRBackgroundService>();
builder.Services.AddHostedService<DonkiCMEBackgroundService>();
builder.Services.AddHostedService<SpaceXBackgroundService>();
builder.Services.AddHostedService<OsdrBackgroundService>();
builder.Services.AddScoped<IOsdrService,OsdrService>();
builder.Services.AddScoped<IOsdrRepository, OsdrRepository>();
builder.Services.AddScoped<ISpaceCacheService, SpaceCacheService>();
builder.Services.AddHttpClient<IssApiService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Frame3 v1");
    c.RoutePrefix = string.Empty; 
});

app.ApplyMigrations();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

