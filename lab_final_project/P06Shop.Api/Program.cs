using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using P06Shop.Api.Data;
using P06Shop.Api.Services;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("P06Shop.Api.Tests")]

namespace P06Shop.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure MongoDB
        builder.Services.AddSingleton<MongoDbContext>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetValue<string>("MongoDB:ConnectionString");
            var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName");
            return new MongoDbContext(connectionString!, databaseName!);
        });

        // Configure Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        // Register services
        builder.Services.AddScoped<AuthService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Only use HTTPS redirection if not in Docker
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
