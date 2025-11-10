

using MauiStoreApp.Services;
using MauiStoreApp.Views;
using OURSTORE.Localization;

namespace MauiStoreApp
{
    public partial class App : Application
    {
        public static LocalizationResourceManager LocalizationResourceManager { get; private set; }
        public Func<object, object, Task> Loaded { get; }
        private readonly AuthService _authService;



        public App(AuthService authService)
        {
            InitializeComponent();

            _authService = authService; // ✅ صح
            //CartService.Instance.LoadCart();

            //authService = authService; // ✅ مهم جداً
            var savedLang = Preferences.Get("AppLanguage", "en");

            LocalizationResourceManager = new LocalizationResourceManager(typeof(AppResources));
            LocalizationResourceManager.SetCulture(savedLang);



        MainPage = new AppShell();


            //MainPage.Loaded += async (s, e) => await CheckUserSession();



            MainPage.Loaded += async (s, e) =>
            {
                try
                {
                    await MauiProgram.SupabaseClient.InitializeAsync();
                    await Task.Delay(800); // ✅ ندي فرصة للـ UI يثبت
                    await CheckUserSession(); // ✅ تشيك بعد الاستقرار
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CheckUserSession Error: {ex.Message}");
                    await Shell.Current.DisplayAlert("Error", "Something went wrong while checking user session.", "OK");
                }
            };


        }


        private async void CheckLoginState()
        {
            var logged = await SecureStorage.GetAsync("isLoggedIn");

            if (logged == "true")
                await Shell.Current.GoToAsync("//HomePage");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }






        private async Task CheckUserSession()
        {
            try
            {
                await MauiProgram.SupabaseClient.InitializeAsync(); // ✅ هنا initialization
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔥 Init Error: {ex}");
            }

            var logged = await SecureStorage.GetAsync("isLoggedIn");

            if (logged != "true")
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            bool restored = false;

            try
            {
                restored = await _authService.TryRestoreFullSessionAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔥 Restore Error: {ex.Message}");
            }

            await Shell.Current.GoToAsync(restored ? "//HomePage" : "//LoginPage");

            //if (restored)
            //    await Shell.Current.GoToAsync("//HomePage");
            //else
            //    await Shell.Current.GoToAsync("//LoginPage");
        }







        public static void SetAppLanguage(string lang)
        {
            try
            {
                var culture = new System.Globalization.CultureInfo(lang);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                OURSTORE.Localization.AppResources.Culture = culture;

                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.FlowDirection =
                        lang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting language: {ex.Message}");
            }
        }












    }
}
