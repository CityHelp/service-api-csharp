using service_api_csharp.Infrastructure;
using DotNetEnv;
using service_api_csharp.Application;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var services = builder.Services;
var configuration = builder.Configuration;

services.AddInfrastructureServices(configuration);
services.AddApplicationDI();

var app = builder.Build();

// builder.Services.AddControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

