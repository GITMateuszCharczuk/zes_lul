using Microsoft.Extensions.Logging;
using MyMauiApp.Services;

namespace MyMauiApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        // Register ProductService
        builder.Services.AddSingleton<IProductService, ProductService>();

        // Register ProductsViewModel
        builder.Services.AddTransient<ViewModels.ProductsViewModel>();

		return builder.Build();
	}
}
