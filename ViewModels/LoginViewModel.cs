//using System.Diagnostics;
//using System.Globalization;
//using System.Net.Http.Json;
//using System.Windows.Input;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using MauiStoreApp.Models;
//using MauiStoreApp.Services;
//using MauiStoreApp.Views;

//namespace MauiStoreApp.ViewModels
//{
//    public partial class LoginViewModel 

//        : BaseViewModel

//    {
//        private readonly AuthService _authService;
//        [ObservableProperty] string username;
//        [ObservableProperty] string password;


//        public LoginViewModel()
//        {
//            _authService = new AuthService();
//        }

//        [RelayCommand]
//        private async Task GoToRegisterPage()
//        {
//            // لازم تستخدم Navigation
//            await Shell.Current.GoToAsync(nameof(RegisterPage));
//        }





//        //[RelayCommand]
//        //private async Task GoToRegister()
//        //{
//        //    await Shell.Current.GoToAsync(nameof(RegisterPage));
//        //}





//        //public LoginViewModel()
//        //{
//        //    _authService = new AuthService();
//        //    LoginCommand = new Command(async () => await LoginAsync());
//        //    GoToRegisterCommand = new Command(async () => await GoToRegisterPage());

//        //}


//        [RelayCommand]
//        private async Task LoginAsync()
//        {
//            if (IsBusy) return;
//            IsBusy = true;

//            try
//            {
//                var result = await _authService.LoginAsync(Username, Password);

//                if (result != null)
//                {
//                    // ✅ لو نجح اللوجين، ندخله على الصفحة الرئيسية
//                    await Shell.Current.GoToAsync("//HomePage");
//                }
//                else
//                {
//                    await Application.Current.MainPage.DisplayAlert("خطأ", "اسم المستخدم أو كلمة المرور غير صحيحة", "حسنًا");
//                }
//            }
//            catch (Exception ex)
//            {
//                await Application.Current.MainPage.DisplayAlert("خطأ", ex.Message, "موافق");
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }



//        //[RelayCommand]
//        //private async Task GoToRegister()
//        //{
//        //    await Shell.Current.GoToAsync(nameof(RegisterPage));
//        //}




















//        //public ICommand GoToRegisterCommand { get; }

//        //public LoginViewModel()
//        //{
//        //    GoToRegisterCommand = new Command(async () => await GoToRegisterPage());
//        //}




//        //[RelayCommand]
//        //public async Task Login()
//        //{
//        //    if (IsBusy)
//        //        return;

//        //    IsBusy = true;

//        //    try
//        //    {
//        //        var loginResponse = await _authService.LoginAsync(Username, Password);

//        //        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
//        //        {
//        //            await Shell.Current.DisplayAlert("تم", "تم تسجيل الدخول بنجاح ✅", "موافق");
//        //            await Shell.Current.GoToAsync("//HomePage");
//        //        }
//        //        else
//        //        {
//        //            await Shell.Current.DisplayAlert("خطأ", "اسم المستخدم أو كلمة المرور غير صحيحة ❌", "حسناً");
//        //        }
//        //    }
//        //    finally
//        //    {
//        //        IsBusy = false;
//        //    }
//        //}



//    }

//}





















using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.Views;

namespace MauiStoreApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] string email;
        [ObservableProperty] string password;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }


        //public ICommand GoToRegisterCommand { get; }
        
        
        [RelayCommand]

        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }





        [RelayCommand]
        public async Task Login()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var success = await _authService.LoginAsync(Email, Password);

                if (success)
                {
                    await Shell.Current.DisplayAlert("تم ✅", "تم تسجيل الدخول بنجاح", "موافق");
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("خطأ ❌", "بيانات الدخول غير صحيحة", "حسناً");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
