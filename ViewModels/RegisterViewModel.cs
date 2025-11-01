

//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using MauiStoreApp.Models;
//using MauiStoreApp.Services;
//using OURSTORE.Models;
//using System.Net.Http.Json;

//namespace MauiStoreApp.ViewModels
//{
//    public partial class RegisterViewModel : BaseViewModel
//    {

//        private readonly AuthService _authService;

//        [ObservableProperty] string username;
//        [ObservableProperty] string email;
//        [ObservableProperty] string password;


//        public RegisterViewModel()
//        {
//            _authService = new AuthService();
//        }





//        [RelayCommand]
//        public async Task Register()
//        {
//            if (IsBusy)
//                return;

//            IsBusy = true;

//            try
//            {
//                var newUser = new UserModel
//                {
//                    username = Username,
//                    email = Email,
//                    password = Password
//                };

//                var success = await _authService.RegisterAsync(newUser);

//                if (success)
//                {
//                    await Shell.Current.DisplayAlert("تم", "تم إنشاء الحساب بنجاح ✅", "موافق");
//                    await Shell.Current.GoToAsync($"//LoginPage");
//                }
//                else
//                {
//                    await Shell.Current.DisplayAlert("خطأ", "فشل التسجيل، حاول مرة أخرى.", "حسناً");
//                }
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }
//    }

//}




































using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;

namespace MauiStoreApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] string email;
        [ObservableProperty] string password;

        public RegisterViewModel()
        {
            _authService = new AuthService();
        }

        [RelayCommand]
        public async Task Register()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var success = await _authService.RegisterAsync(Email, Password);

                if (success)
                {
                    await Shell.Current.DisplayAlert("تم ✅", "تم إنشاء الحساب بنجاح!", "موافق");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("خطأ ❌", "حدث خطأ أثناء التسجيل", "حسناً");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
