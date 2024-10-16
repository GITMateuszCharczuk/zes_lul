using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Dodaj us�ugi MVC do kontenera DI (Dependency Injection)
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Konfiguracja potoku HTTP
if (!app.Environment.IsDevelopment())
{
    // Obs�uga b��d�w w �rodowisku produkcyjnym
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Obs�uga HSTS dla bezpiecznego HTTPS
}

app.UseHttpsRedirection();  // Przekierowanie na HTTPS
app.UseStaticFiles();  // Obs�uga plik�w statycznych (np. CSS, JS)

app.UseRouting();  // W��czenie routingu

app.UseSession();  // Add this line

app.UseAuthorization();  // W��czenie autoryzacji (je�li potrzebna)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Index}/{id?}");  // Konfiguracja domy�lnej trasy

app.Run();  // Uruchomienie aplikacji
