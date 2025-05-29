using dotenv.net;
using SCED.API;
using SCED.API.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

Settings.Initialize(builder.Configuration);

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddControllerServices();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddCorsServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();