using service_api_csharp.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using service_api_csharp.API.Authentication;
using service_api_csharp.Application;
using service_api_csharp.API.Extensions;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers(); // Agregar soporte para controllers

builder.Services.AddAuthentication("CustomJwtScheme")
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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ========== JWT Authentication Middleware ==========
// IMPORTANTE: Este middleware debe estar ANTES de los endpoints que quieres proteger
app.UseAuthentication();
app.UseAuthorization();


// Mapear los controllers
app.MapControllers();

app.Run();

