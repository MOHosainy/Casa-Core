







using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Services;
using MauiStoreApp.Views;

namespace MauiStoreApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty] string email;
        [ObservableProperty] string password;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
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
                var success = await _authService.RegisterAsync(Email, Password);

                if (success)
                {
                    await Shell.Current.DisplayAlert("تم ✅", "تم إنشاء الحساب بنجاح!", "موافق");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    //await Shell.Current.DisplayAlert("خطأ ❌", "حدث خطأ أثناء التسجيل", "حسناً");
                }
            }
            finally
            {
                IsBusy = false;
            }
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
