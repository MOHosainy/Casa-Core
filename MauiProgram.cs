using CommunityToolkit.Maui;
using MauiStoreApp.Services;
using MauiStoreApp.ViewModels;
using MauiStoreApp.Views;
using Microsoft.Extensions.Logging;
using MauiStoreApp.CustomControls.Borderless; // لازم تضيفه فوق كمان
using Microsoft.Maui.Handlers;

namespace MauiStoreApp
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















            builder.Services.AddSingleton<BaseService>();
            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddSingleton<CategoryService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<RecentlyViewedProductsService>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ProductDetailsViewModel>();
            builder.Services.AddTransient<ProductDetailsPage>();
            builder.Services.AddTransient<CategoryPageViewModel>();
            builder.Services.AddTransient<CategoryPage>();
            builder.Services.AddTransient<RecentlyViewedPageViewModel>();
            builder.Services.AddTransient<RecentlyViewedPage>();
            builder.Services.AddTransient<CartViewModel>();
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
