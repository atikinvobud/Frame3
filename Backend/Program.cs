using Backend.Models;
using BackEnd.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
Env.Load();
var builder = WebApplication.CreateBuilder(args);

var dbHost = builder.Configuration["DB_HOST"];
var dbPort = builder.Configuration["DB_PORT"];
var dbName = builder.Configuration["DB_NAME"];
var dbUser = builder.Configuration["DB_USER"];
var dbPassword = builder.Configuration["DB_PASSWORD"];
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
builder.Services.AddDbContext<Context>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseHttpsRedirection();

app.ApplyMigrations();
app.Run();

