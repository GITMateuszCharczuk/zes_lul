using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Dodaj us³ugi MVC do kontenera DI (Dependency Injection)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja potoku HTTP
if (!app.Environment.IsDevelopment())
{
    // Obs³uga b³êdów w œrodowisku produkcyjnym
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Obs³uga HSTS dla bezpiecznego HTTPS
}

app.UseHttpsRedirection();  // Przekierowanie na HTTPS
app.UseStaticFiles();  // Obs³uga plików statycznych (np. CSS, JS)

app.UseRouting();  // W³¹czenie routingu

app.UseAuthorization();  // W³¹czenie autoryzacji (jeœli potrzebna)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}/{id?}");  // Konfiguracja domyœlnej trasy

app.Run();  // Uruchomienie aplikacji
