//namespace MauiStoreApp
//{
//    public partial class App : Application
//    {
//        public App()
//        {
//            InitializeComponent();

//            MainPage = new AppShell();
//            Shell.Current.GoToAsync("//LoginPage");

//        }
//    }
//}

using MauiStoreApp.Views;
using OURSTORE.Localization;
//using MauiStoreApp.Localization; // تأكد من وجود هذا الـ namespace

namespace MauiStoreApp
{
    public partial class App : Application
    {
        //public static object LocalizationResourceManager { get; internal set; }
        public static LocalizationResourceManager LocalizationResourceManager { get; private set; }
        //public static LocalizationResourceManager LocalizationResourceManager { get; } = new LocalizationResourceManager();


        public App()
        {
            InitializeComponent();


            // تحميل اللغة المحفوظة أو الإنجليزية كافتراضية
            var savedLang = Preferences.Get("AppLanguage", "en");

            LocalizationResourceManager = new LocalizationResourceManager(typeof(AppResources));
            LocalizationResourceManager.SetCulture(savedLang);



            MainPage = new AppShell();

            // ✅ نخلي الانتقال يحصل بعد ما الـ Shell يجهز
            //GoToLoginPage();
            MainPage.Dispatcher.Dispatch(async () => await GoToLoginPage());

        }

        private async Task GoToLoginPage()
        {
            try
            {
                await Task.Delay(300); // اختياري فقط لراحة التحميل
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation Error: {ex}");
                await Application.Current.MainPage.DisplayAlert("Navigation Error", ex.Message, "OK");
            }
        }





        //private async void GoToLoginPage()
        //{
        //    try
        //    {
        //        // ننتظر شوية لحد ما الـ Shell يتهيأ بالكامل
        //        await Task.Delay(300);

        //        await Shell.Current.GoToAsync("//LoginPage");
        //    }
        //    catch (Exception ex)
        //    {
        //        // useful for debugging if it still fails silently
        //        System.Diagnostics.Debug.WriteLine($"Navigation Error: {ex.Message}");
        //    }
        //}
    }
}
