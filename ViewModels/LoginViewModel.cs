
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.Views;

namespace MauiStoreApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        bool isPasswordVisible;




        [ObservableProperty]
        string currentLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;



        public LoginViewModel(AuthService authService)
        {
            _authService = authService;

            //_authService = new AuthService(); // dummy service لو مش موجود

            // تعيين اللغة المحفوظة أو الإنجليزية افتراضيًا
            CurrentLang = Preferences.Get("AppLanguage", "en");
        }



        [RelayCommand]
        private void ChangeLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang) || lang == CurrentLang)
                return;

            CurrentLang = lang;
            Preferences.Set("AppLanguage", lang);

            // تحديث الثقافة في التطبيق
            App.LocalizationResourceManager.SetCulture(lang); // ✅ هنا يجب أن يكون string

            // تحديث الخاصية
            //CurrentLang = lang;

            // إعادة تحميل الصفحة الرئيسية أو الـ Shell
            Application.Current.MainPage = new AppShell();
        }



        public LoginViewModel()
        {
            // dummy service لو مش متاح
            _authService = new AuthService();
        }

       

      



        public ICommand GoToRegisterCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        });


        [RelayCommand]
        public async Task Login()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await App.Current.MainPage.DisplayAlert("خطأ", "من فضلك ادخل الإيميل وكلمة المرور", "حسناً");
                    return;
                }

                // ✅ جِب بيانات المستخدم المسجل مسبقًا (لو فيه)
                var savedEmail = await SecureStorage.Default.GetAsync("registered_email");
                var savedPassword = await SecureStorage.Default.GetAsync("registered_password");

                // ✅ السماح بالدخول إذا:
                // 1. استخدم الإيميل والباسورد المسجلين
                // 2. أو استخدم الإيميل الافتراضي (مثلاً admin@example.com)
                if ((Username == savedEmail && Password == savedPassword) ||
                    (Username == "admin@example.com" && Password == "1234"))
                {
                    await SecureStorage.Default.SetAsync("token", "dummy_token_123");
                    await SecureStorage.Default.SetAsync("userId", "1");

                    var toast = CommunityToolkit.Maui.Alerts.Toast.Make(
                        "تم تسجيل الدخول بنجاح ✅",
                        CommunityToolkit.Maui.Core.ToastDuration.Short,
                        14);

                    await toast.Show();

                    await Shell.Current.GoToAsync("//MainTabs");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("خطأ", "البريد الإلكتروني أو كلمة المرور غير صحيحة", "حسناً");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("خطأ", ex.Message, "حسناً");
            }
            finally
            {
                IsBusy = false;
            }
        }








        [RelayCommand]
        public void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
