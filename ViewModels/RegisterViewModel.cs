
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.Views;
using OURSTORE.Localization;
using System.Globalization;

namespace MauiStoreApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] string email;
        [ObservableProperty] string password;

        //[ObservableProperty]
        //private string currentLang = Preferences.Get("AppLanguage", "ar");
        private string _currentLang = Preferences.Get("AppLanguage", "ar");


        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            ApplyLanguage(CurrentLang);

        }



        [RelayCommand]
        private async Task GoToLogin()
        {
            //await Shell.Current.GoToAsync(nameof(LoginPage));
            await Shell.Current.GoToAsync("//LoginPage");

        }






        [RelayCommand]
        public async Task Register()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {


                Email = Email?.Trim().ToLowerInvariant();
                Password = Password?.Trim();


                var success = await _authService.RegisterAsync(Email, Password);

                if (success)
                {

                    //await Shell.Current.DisplayAlert("تم ✅", "تم إنشاء الحساب بنجاح!", "موافق");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    //var toast = Toast.Make("✅ تم إنشاء الحساب بنجاح", ToastDuration.Short);

                    // الحساب موجود
                    //await Shell.Current.DisplayAlert("تنبيه ⚠️", "هذا البريد مسجّل بالفعل، سيتم تسجيل الدخول تلقائيًا", "موافق");
                    var loginResult = await _authService.LoginAsync(Email, Password);
                    if (loginResult)
                    {
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                }
            }
            catch (Exception ex)
            {
                // فقط الأخطاء الحقيقية التي ليست بسبب الإيميل موجود
                if (!ex.Message.Contains("already registered"))
                {
                    //await Shell.Current.DisplayAlert("خطأ ❌", "حدث خطأ أثناء التسجيل", "حسناً");
                    System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
                }
                // إذا كانت رسالة الاستثناء عن الإيميل الموجود، لا نفعل شيء
            }
            finally
            {
                IsBusy = false;
            }
        }






        //public string CurrentLang { get; private set; }



        //public string CurrentLanguage { get; private set; }


        //private void ChangeLanguages(string lang)
        //{
        //    CurrentLang = lang;
        //    Preferences.Set("AppLanguage", lang);
        //    App.LocalizationResourceManager.SetCulture(lang);

        //    // أنشئ AppShell جديد
        //    var newShell = new AppShell();

        //    // ضع MainPage الجديد
        //    Application.Current.MainPage = newShell;

        //    // الآن ضع FlowDirection للصفحة الجديدة
        //    Application.Current.MainPage.FlowDirection = lang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        //    // بلّغ XAML أي Binding على CurrentFlowDirection
        //    OnPropertyChanged(nameof(CurrentFlowDirection));
        //}


        //private void ApplyLanguage(string lang)
        //{
        //    var culture = new CultureInfo(lang);
        //    Thread.CurrentThread.CurrentCulture = culture;
        //    Thread.CurrentThread.CurrentUICulture = culture;
        //    AppResources.Culture = culture;

        //    Application.Current.MainPage.FlowDirection =
        //        lang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        //    OnPropertyChanged(nameof(CurrentLang));
        //}



        ////private string _currentLanguage = "en"; // أو "ar"
        //public FlowDirection CurrentFlowDirection
        //{
        //    //get => CurrentLanguage == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        //    get => CurrentLang == "ar" ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

        //}






















        private void ApplyLanguage(string lang)
        {
            var culture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            AppResources.Culture = culture;

            Application.Current.MainPage.FlowDirection =
                lang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            OnPropertyChanged(nameof(CurrentLang));
        }

        //private string _currentLang = Preferences.Get("AppLanguage", "ar");
        public string CurrentLang
        {
            get => _currentLang;
            set
            {
                if (SetProperty(ref _currentLang, value))
                    OnPropertyChanged(nameof(CurrentFlowDirection));
            }
        }

        public FlowDirection CurrentFlowDirection =>
            CurrentLang == "ar" ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

        //CurrentLang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        [RelayCommand]


        private void ChangeLanguages(string lang)
        {
            CurrentLang = lang;
            Preferences.Set("AppLanguage", lang);

            //ApplyLanguage(CurrentLang);

            App.LocalizationResourceManager.SetCulture(lang);
            // 🔄 Reload UI
            //Application.Current.MainPage = new AppShell();

            Application.Current.MainPage.FlowDirection =
       lang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // 🔄 بلّغ الواجهة إن الاتجاه اتغير
            OnPropertyChanged(nameof(CurrentFlowDirection));

            // ⚡ تحديث الواجهة (اختياري لو عندك صفحات ثابتة)
            Application.Current.MainPage = new AppShell();
        }

























































        [ObservableProperty]
        private bool isPasswordVisible = false;

        public string PasswordEntryText => isPasswordVisible ? "Text" : "Password";


        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
            OnPropertyChanged(nameof(PasswordEntryText));
        }


    }
}
