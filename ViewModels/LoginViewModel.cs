

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.Views;
using OURSTORE.Localization;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace MauiStoreApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] string email;
        [ObservableProperty] string password;

        [ObservableProperty]
        private string currentLang = Preferences.Get("AppLanguage", "ar");


        //public string CurrentLang { get; private set; }

        public LoginViewModel(AuthService authService)
        {
            //_authService = new AuthService();
            _authService = authService;

            ApplyLanguage(CurrentLang);
        }
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
            get => CurrentLang == "ar" ? FlowDirection.LeftToRight : FlowDirection.RightToLeft ;

        }














        [RelayCommand]

        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }




        [RelayCommand]

        private async Task Login()
        {
            try
            {

                Email = Email?.Trim().ToLowerInvariant();
                Password = Password?.Trim();

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert(" (Alert) تنبيه", " (Please enter your email and password) من فضلك أدخل البريد وكلمة المرور", " (OK) موافق");
                    return;
                }

                IsBusy = true;

                
                var result = await _authService.LoginAsync(Email, Password);
                //CartService.Instance.LoadCart();



                if (Email == "tester@example.com" && Password == "Tester@1234")
                {
                    // انتقل مباشرةً للـ HomePage (تجاوز التحقق)
                    await Shell.Current.GoToAsync("//HomePage");
                    return;
                }



                if (!result)
                {


                    await Shell.Current.DisplayAlert("خطأ", "اسم المستخدم أو كلمة المرور غير صحيحة ‼️", "موافق");
                    return;
                }
                //await _authService.StoreSessionAsync(result.Session);
                var session = _authService.CurrentSession;
                await _authService.StoreSessionAsync(session);


                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("خطأ", "Login Error: " + ex.Message, "موافق");
            }
            finally
            {
                IsBusy = false;
            }
        }
















        //[RelayCommand]
        //private async Task Login()
        //{
        //    try
        //    {

        //        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        //        {
        //            await Shell.Current.DisplayAlert("تنبيه", "من فضلك أدخل البريد وكلمة المرور", "موافق");
        //            return;
        //        }
        //        IsBusy = true;


        //        if (Email == "tester@example.com" && Password == "Tester@1234")
        //        {
        //            // انتقل مباشرةً للـ HomePage (تجاوز التحقق)
        //            await Shell.Current.GoToAsync("//HomePage");
        //            return;
        //        }

        //        // نحاول تسجيل الدخول
        //        var result = await _authService.LoginAsync(Email, Password);

        //        // لو الدخول فشل، نعرض رسالة خطأ
        //        if (!result)
        //        {
        //            await Shell.Current.DisplayAlert("خطأ", "اسم المستخدم أو كلمة المرور غير صحيحة ‼️", "موافق");
        //            return;
        //        }

        //        // الدخول ناجح → روح للصفحة الرئيسية
        //        await Shell.Current.GoToAsync("//HomePage");
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("خطأ", "Login Error: " + ex.Message, "موافق");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}








































        [ObservableProperty]
        private bool isPasswordVisible = false;

        public string PasswordEntryText => isPasswordVisible ? "Text" : "Password";

        public string CurrentLanguage { get; private set; }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
            OnPropertyChanged(nameof(PasswordEntryText));
        }



    }
}
