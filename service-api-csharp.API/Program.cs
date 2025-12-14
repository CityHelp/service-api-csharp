using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using service_api_csharp.API.Authentication;
using service_api_csharp.Infrastructure;
using service_api_csharp.Application;

var builder = WebApplication.CreateBuilder();

// Configuration loading order: appsettings.json -> environment variables
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

//generic things
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "CustomJwtScheme";
    options.DefaultChallengeScheme = "CustomJwtScheme";
})
    .AddScheme<AuthenticationSchemeOptions, CustomJwtHandler>("CustomJwtScheme", null);

builder.Services.AddAuthorization();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddInfrastructureServices(configuration);
services.AddApplicationServices();
    
var app = builder.Build();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Healthcheck
app.MapMethods("/health", new[] { "HEAD" }, () => Results.Ok());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Mapear los controllers
app.MapControllers();

app.Run();

