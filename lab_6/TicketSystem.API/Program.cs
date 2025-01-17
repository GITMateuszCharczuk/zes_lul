using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using TicketSystem.API.Hubs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Get MongoDB connection settings from environment variables
var mongoConnectionString = Environment.GetEnvironmentVariable("MongoDB__ConnectionString") 
    ?? builder.Configuration["MongoDB:ConnectionString"];
var mongoDatabaseName = Environment.GetEnvironmentVariable("MongoDB__DatabaseName") 
    ?? builder.Configuration["MongoDB:DatabaseName"];

// Add MongoDB
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase(mongoDatabaseName);
builder.Services.AddSingleton<IMongoDatabase>(database);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder
            .WithOrigins("http://localhost", "http://localhost:80")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();
app.MapHub<TicketHub>("/tickethub");

// Set the server URL and port
app.Urls.Add("http://0.0.0.0:8080");

// Log MongoDB connection details
Console.WriteLine($"MongoDB Connection String: {mongoConnectionString}");
Console.WriteLine($"MongoDB Database Name: {mongoDatabaseName}");

app.Run();
