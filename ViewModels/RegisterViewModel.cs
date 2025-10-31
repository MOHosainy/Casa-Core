////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace OURSTORE.ViewModels
////{
////    class RegisterViewModel
////    {
////    }
////}

//using System.Windows.Input;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;

//namespace MauiStoreApp.ViewModels;

//public partial class RegisterViewModel : ObservableObject
//{
//    [ObservableProperty] string username;
//    [ObservableProperty] string email;
//    [ObservableProperty] string password;

//    public ICommand RegisterCommand => new AsyncRelayCommand(RegisterAsync);
//    public ICommand GoToLoginCommand => new AsyncRelayCommand(GoToLoginAsync);

//    private async Task RegisterAsync()
//    {
//        // 🔹 مؤقتًا: نقبل أي بيانات
//        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
//        {
//            await Shell.Current.DisplayAlert("Error", "Please fill all fields", "OK");
//            return;
//        }

//        // 🔹 ممكن تخزنها مؤقتًا في Preferences (لو عايز تسجل الدخول بيها بعدين)
//        Preferences.Set("Username", Username);
//        Preferences.Set("Password", Password);

//        await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
//        await Shell.Current.GoToAsync("//LoginPage");
//    }

//    private async Task GoToLoginAsync()
//    {
//        await Shell.Current.GoToAsync("//LoginPage");
//    }
//}



using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiStoreApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty] private string username;
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;

        [ObservableProperty]
        private bool isPasswordVisible;

        [ObservableProperty] private string confirmPassword;


        public bool IsPasswordHidden => !IsPasswordVisible;
        [ObservableProperty] private bool isConfirmPasswordVisible;

        public ICommand RegisterCommand => new AsyncRelayCommand(RegisterAsync);
        public ICommand GoToLoginCommand => new AsyncRelayCommand(GoToLoginAsync);


        public RegisterViewModel()
        {
            IsPasswordVisible = false; // الافتراضي مخفي
            IsConfirmPasswordVisible = false;
        }


        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
            OnPropertyChanged(nameof(IsPasswordHidden));
        }


        [RelayCommand]
        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }


        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)

                 ||
                string.IsNullOrWhiteSpace(ConfirmPassword)  
                )
            {
                await Shell.Current.DisplayAlert("خطأ", "من فضلك ادخل الإيميل وكلمة المرور", "تمام");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("خطأ", "كلمة المرور وتأكيدها غير متطابقين", "تمام");
                return;
            }



            // ✅ احفظ بيانات المستخدم المسجل مؤقتًا
            await SecureStorage.Default.SetAsync("registered_email", Email);
            await SecureStorage.Default.SetAsync("registered_password", Password);

            await Shell.Current.DisplayAlert("تم التسجيل", "تم إنشاء الحساب بنجاح ✅", "تسجيل الدخول");

            // بعد التسجيل ينتقل لصفحة اللوجين
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
