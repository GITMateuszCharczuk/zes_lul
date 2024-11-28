using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using P06Shop.Api.Data;
using P06Shop.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<MongoDbContext>(sp =>
    new MongoDbContext("mongodb://localhost:27017", "P06ShopDb"));

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
