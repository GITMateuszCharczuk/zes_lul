using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Set the environment variable for ASP.NET Core
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
builder.Environment.EnvironmentName = environment;

// Set the URLs to listen on
// builder.WebHost.UseUrls("http://localhost:5000");

// Read the MongoDB connection string from the environment variable
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_DB_CONNECTION_STRING") 
                            ?? "mongodb://localhost:27017"; // Fallback to localhost if not set

// Add services to the container.
builder.Services.AddSingleton<MongoDbContext>(sp =>
    new MongoDbContext(mongoConnectionString, "P06ShopDb"));

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MVC services
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint
    app.UseSwaggerUI(c => // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "P06Shop API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseCors("AllowAllOrigins");

// Map controllers
app.MapControllers();

app.Run();
