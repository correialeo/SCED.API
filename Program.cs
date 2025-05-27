using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using SCED.API;
using SCED.API.Infrasctructure.Context;
using SCED.API.Interfaces;
using SCED.API.Services;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IServiceCollection? services = builder.Services;

Settings.Initialize(builder.Configuration);

string connectionString = Settings.GetConnectionString();

services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
    {
        options.DisableBackslashEscaping();
        options.EnableRetryOnFailure();
    });
});

services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

services.AddScoped<DeviceDataService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IShelterService, ShelterService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IAlertService, AlertService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
