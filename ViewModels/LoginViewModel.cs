using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Models;
using MauiStoreApp.Services;
using MauiStoreApp.Views;

namespace MauiStoreApp.ViewModels
{
    public partial class LoginViewModel 
        
        : BaseViewModel

    {


        private async Task GoToRegisterPage()
        {
            // لازم تستخدم Navigation
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }




        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;
        private readonly AuthService _authService;


        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () => await GoToRegisterPage());

        }



        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var result = await _authService.LoginAsync(Username, Password);

                if (result != null)
                {
                    // ✅ لو نجح اللوجين، ندخله على الصفحة الرئيسية
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("خطأ", "اسم المستخدم أو كلمة المرور غير صحيحة", "حسنًا");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("خطأ", ex.Message, "موافق");
            }
            finally
            {
                IsBusy = false;
            }
        }























        public ICommand GoToRegisterCommand { get; }

        //public LoginViewModel()
        //{
        //    GoToRegisterCommand = new Command(async () => await GoToRegisterPage());
        //}



        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("خطأ", "أدخل اسم المستخدم وكلمة المرور", "حسناً");
                return;
            }

            try
            {
                var request = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                using var client = new HttpClient();
                var response = await client.PostAsJsonAsync("https://fakestoreapi.com/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (!string.IsNullOrEmpty(result?.Token))
                    {
                        await App.Current.MainPage.DisplayAlert("نجاح ✅", "تم تسجيل الدخول بنجاح", "تمام");
                        await Shell.Current.GoToAsync("//MainTabs");
                        return;
                    }
                }

                // ✅ لو السيرفر رفض، نتحقق محليًا
                var localUsername = await SecureStorage.Default.GetAsync("username");
                var localPassword = await SecureStorage.Default.GetAsync("password");

                if (Username == localUsername && Password == localPassword)
                {
                    await App.Current.MainPage.DisplayAlert("نجاح محلي ✅", "تم تسجيل الدخول من التخزين المحلي", "تمام");
                    await Shell.Current.GoToAsync("//MainTabs");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("خطأ", "بيانات الدخول غير صحيحة", "حسناً");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("خطأ", ex.Message, "حسناً");
            }
        }
    }

}

