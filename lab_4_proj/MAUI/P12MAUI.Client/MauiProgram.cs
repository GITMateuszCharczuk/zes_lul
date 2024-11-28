using Microsoft.Extensions.Logging;
using P06Shop.Shared.Confguration;
using P06Shop.Shared.MessageBox;
using P06Shop.Shared.Services.ProductService;
using P12MAUI.Client.MessageBox;
using P12MAUI.Client.ViewModels;
namespace P12MAUI.Client
{
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            ConfigureServices(builder.Services);
            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var appSettings = ConfigureAppSettings(services);
            ConfigureAppServices(services, appSettings);
            ConfigureViewModels(services);
            ConfigureViews(services);
            ConfigureHttpClients(services, appSettings);
        }

        private static void ConfigureAppServices(IServiceCollection services, AppSettings appSettings)
        {
            services.AddSingleton<IConnectivity>(Connectivity.Current);
            services.AddSingleton<IGeolocation>(Geolocation.Default);
            services.AddSingleton<IMap>(Map.Default);
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IMessageDialogService, MauiMessageDialogService>();
           
        }

        private static void ConfigureViewModels(IServiceCollection services)
        {
            // tutaj konfigurujemy viewmodele
        
            services.AddSingleton<ProductsViewModel>();
            services.AddSingleton<ProductDetailsViewModel>();
        }

        private static void ConfigureViews(IServiceCollection services)
        {
            // tutaj konfigurujemy widoki
            services.AddSingleton<MainPage>();
            services.AddTransient<ProductDetailsView>();
        }

        private static void ConfigureHttpClients(IServiceCollection services, AppSettings appSettings)
        {
            // tutaj konfigurujemy HttpClient

            var urliBulder = new UriBuilder(appSettings.BaseApiUrl)
            {
                Path = appSettings.ProductEndpoint.BaseUrl
            };

            //żeby skonfigurować HttpClienta, musimy dodać pakiet Microsoft.Extensions.Http
            services.AddHttpClient<IProductService, ProductService>(client => client.BaseAddress = urliBulder.Uri);
        }

        private static AppSettings ConfigureAppSettings(IServiceCollection services)
        {
            //pobranie ustawień z pliku konfiguracyjnego
            // i zmapowanie ich na obiekt AppSettings
            //potrzebujemy pakietu Microsoft.Extensions.Options.ConfigurationExtensions

            //var appSettingsSection = _configuration.GetSection(nameof(AppSettings));
            //var settings = appSettingsSection.Get<AppSettings>();
            //services.Configure<AppSettings>(appSettingsSection); // zarejestrowanie AppSettings w kontenerze DI
            //return settings;

            var appSettingsSection = new AppSettings()
            {
                BaseApiUrl = "https://cd8c-2a02-a319-ee-a480-41e6-c52b-a0c1-9def.ngrok-free.app",
                ProductEndpoint = new ProductEndpoint()
                {
                    BaseUrl = "/whatever/api/",
                    GetProducts = "Products",
                    CreateProduct = "Products",
                    UpdateProduct = "Products",
                    SearchProducts = "Products",
                    DeleteProducts = "Products",
                }
            };

            services.Configure<AppSettings>(options =>
            {
                options.BaseApiUrl = appSettingsSection.BaseApiUrl;
                options.ProductEndpoint = appSettingsSection.ProductEndpoint;
            });
            return appSettingsSection;



        }
    }
}
