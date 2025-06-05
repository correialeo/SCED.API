using dotenv.net;
using Microsoft.EntityFrameworkCore;
using SCED.API;
using SCED.API.Extensions;
using SCED.API.Infrasctructure.Context;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

Settings.Initialize(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddControllerServices();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddCorsServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.ConfigurePipeline();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate(); 
}

app.Run();