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

namespace MauiStoreApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // ✅ نخلي الانتقال يحصل بعد ما الـ Shell يجهز
            GoToLoginPage();
        }
             
        private async void GoToLoginPage()
        {
            try
            {
                // ننتظر شوية لحد ما الـ Shell يتهيأ بالكامل
                await Task.Delay(300);

                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                // useful for debugging if it still fails silently
                System.Diagnostics.Debug.WriteLine($"Navigation Error: {ex.Message}");
            }
        }
    }
}
