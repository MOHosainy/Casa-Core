
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.Views;
using OURSTORE.Localization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MauiStoreApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] string email;
        [ObservableProperty] string password;

        [ObservableProperty]
        private string currentLang = Preferences.Get("AppLanguage", "ar");


        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            CurrentLang = Preferences.Get("AppLanguage", "ar");

            ApplyLanguage(CurrentLang);

        }



        [RelayCommand]
        private async Task GoToLogin()
        {
            //await Shell.Current.GoToAsync(nameof(LoginPage));
            await Shell.Current.GoToAsync("//LoginPage");

        }













private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
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

                if (!IsValidEmail(Email))
                {
                    await Shell.Current.DisplayAlert("خطأ(error) ⚠️", "(Email is invalid) البريد الإلكتروني غير صالح.", "موافق(OK)");
                    return;
                }


                var success = await _authService.RegisterAsync(Email, Password);

                if (success)
                {
                    await Shell.Current.DisplayAlert("تم(Done) ✅", "(The account has been successfully created)تم إنشاء الحساب بنجاح!", "موافق(Ok)");
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    var loginSuccess = await _authService.LoginAsync(Email, Password);

                    if (loginSuccess)
                    {
                        await Shell.Current.DisplayAlert(" (Wellcome) مرحباً👋", "(Login successful!) تم تسجيل الدخول بنجاح!", "(continuation) استمرار");
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                    else
                    {
                        //await Shell.Current.DisplayAlert("تنبيه ⚠️", "هذا البريد مسجل بالفعل، لكن كلمة المرور غير صحيحة.", "موافق");
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


        private void ChangeLanguages(string lang)
        {
            CurrentLang = lang;
            Preferences.Set("AppLanguage", lang);
            App.LocalizationResourceManager.SetCulture(lang);

            // أنشئ AppShell جديد
            var newShell = new AppShell();

            // ضع MainPage الجديد
            Application.Current.MainPage = newShell;

            // الآن ضع FlowDirection للصفحة الجديدة
            Application.Current.MainPage.FlowDirection = lang == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // بلّغ XAML أي Binding على CurrentFlowDirection
            OnPropertyChanged(nameof(CurrentFlowDirection));
        }


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



        //private string _currentLanguage = "en"; // أو "ar"
        public FlowDirection CurrentFlowDirection
        {
            //get => CurrentLanguage == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            get => CurrentLang == "ar" ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

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
