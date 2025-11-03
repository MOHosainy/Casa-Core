

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

            //authService = authService; // ✅ مهم جداً
            var savedLang = Preferences.Get("AppLanguage", "en");

            LocalizationResourceManager = new LocalizationResourceManager(typeof(AppResources));
            LocalizationResourceManager.SetCulture(savedLang);



        MainPage = new AppShell();

            
            MainPage.Loaded += async (s, e) => await CheckUserSession();


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
            var logged = await SecureStorage.GetAsync("isLoggedIn");

            if (logged != "true")
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            bool restored = await _authService.TryRestoreFullSessionAsync();

            if (restored)
                await Shell.Current.GoToAsync("//HomePage");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }



       



    }
}
