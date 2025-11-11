using CommunityToolkit.Maui;
using MauiStoreApp.Services;
using Supabase;
using MauiStoreApp.ViewModels;
using MauiStoreApp.Views;
using Microsoft.Extensions.Logging;
using MauiStoreApp.CustomControls.Borderless; // لازم تضيفه فوق كمان
using Microsoft.Maui.Handlers;
//using OURSTORE.ViewModels;

namespace MauiStoreApp
{
    public static class MauiProgram
    {
        public static Supabase.Client SupabaseClient { get; private set; }
        public static object ServiceProvider { get; internal set; }

        public const string SupabaseUrl = "https://phbarflogerpotdqiwrp.supabase.co";
        public const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBoYmFyZmxvZ2VycG90ZHFpd3JwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjE5NDY1NjksImV4cCI6MjA3NzUyMjU2OX0.FUC7B6BJFWcFl-w2I2CLjkLb3YyCVzlCrR9tKEdyJ5M";


        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    //fonts.AddFont("AstoreEczarSemiBold.ttf", "AstoreEczarSemiBold");
                    fonts.AddFont("astore-eczar-semi-bold.ttf", "astore-eczar-semi-bold");
                }).UseMauiCommunityToolkit();

#if ANDROID
            EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
                if (view is BorderlessEntry)
                {
                    handler.PlatformView.Background = null;
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
            });
#endif


            SupabaseClient = new Supabase.Client(
              "https://phbarflogerpotdqiwrp.supabase.co",
              "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBoYmFyZmxvZ2VycG90ZHFpd3JwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjE5NDY1NjksImV4cCI6MjA3NzUyMjU2OX0.FUC7B6BJFWcFl-w2I2CLjkLb3YyCVzlCrR9tKEdyJ5M",
              new SupabaseOptions
              {
                  //AutoRefreshToken = true,
                  AutoRefreshToken = true,
                  //PersistSession = true,
                  //SessionHandler = new Supabase.Session.Options.SessionMemoryHandler() // مهم جدًا
                  //PersistSession = true
              });


            //await SupabaseClient.InitializeAsync();
            //_ = SupabaseClient.InitializeAsync();







            builder.Services.AddSingleton<BaseService>();
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddSingleton<CategoryService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<UserService>();
            //builder.Services.AddSingleton<AuthService>();
            //builder.Services.AddSingleton<CartViewModel>();

            
            builder.Services.AddSingleton<AuthService>();

            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddTransient<RegisterViewModel>();

            builder.Services.AddTransient<CheckoutPage>();
            builder.Services.AddTransient<CheckoutViewModel>();


            builder.Services.AddTransient<CartViewModel>();
            
            builder.Services.AddSingleton<RecentlyViewedProductsService>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ProductDetailsViewModel>();
            builder.Services.AddTransient<ProductDetailsPage>();
            builder.Services.AddTransient<CategoryPageViewModel>();
            builder.Services.AddTransient<CategoryPage>();
            builder.Services.AddTransient<RecentlyViewedPageViewModel>();
            builder.Services.AddTransient<RecentlyViewedPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<ProfilePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
    