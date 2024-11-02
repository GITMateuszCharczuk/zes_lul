var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProductModel}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "create",
    pattern: "create",
    defaults: new { controller = "ProductModel", action = "Create" });

app.MapControllerRoute(
    name: "edit",
    pattern: "edit",
    defaults: new { controller = "ProductModel", action = "Edit" });

app.MapControllerRoute(
    name: "create",
    pattern: "delete",
    defaults: new { controller = "ProductModel", action = "Delete" });

app.Run();
