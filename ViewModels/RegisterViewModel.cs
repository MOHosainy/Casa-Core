
//using System.Threading.Tasks;
//using System.Windows.Input;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;

//namespace MauiStoreApp.ViewModels
//{
//    public partial class RegisterViewModel : ObservableObject
//    {
//        [ObservableProperty] private string username;
//        [ObservableProperty] private string email;
//        [ObservableProperty] private string password;

//        [ObservableProperty]
//        private bool isPasswordVisible;

//        [ObservableProperty] private string confirmPassword;


//        public bool IsPasswordHidden => !IsPasswordVisible;
//        [ObservableProperty] private bool isConfirmPasswordVisible;

//        public ICommand RegisterCommand => new AsyncRelayCommand(RegisterAsync);
//        public ICommand GoToLoginCommand => new AsyncRelayCommand(GoToLoginAsync);


//        public RegisterViewModel()
//        {
//            IsPasswordVisible = false; // الافتراضي مخفي
//            IsConfirmPasswordVisible = false;
//        }


//        [RelayCommand]
//        private void TogglePasswordVisibility()
//        {
//            IsPasswordVisible = !IsPasswordVisible;
//            OnPropertyChanged(nameof(IsPasswordHidden));
//        }


//        [RelayCommand]
//        private void ToggleConfirmPasswordVisibility()
//        {
//            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
//        }


//        private async Task RegisterAsync()
//        {
//            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)

//                 ||
//                string.IsNullOrWhiteSpace(ConfirmPassword)
//                )
//            {
//                await Shell.Current.DisplayAlert("خطأ", "من فضلك ادخل الإيميل وكلمة المرور", "تمام");
//                return;
//            }

//            if (Password != ConfirmPassword)
//            {
//                await Shell.Current.DisplayAlert("خطأ", "كلمة المرور وتأكيدها غير متطابقين", "تمام");
//                return;
//            }



//            // ✅ احفظ بيانات المستخدم المسجل مؤقتًا
//            await SecureStorage.Default.SetAsync("registered_email", Email);
//            await SecureStorage.Default.SetAsync("registered_password", Password);

//            await Shell.Current.DisplayAlert("تم التسجيل", "تم إنشاء الحساب بنجاح ✅", "تسجيل الدخول");

//            // بعد التسجيل ينتقل لصفحة اللوجين
//            await Shell.Current.GoToAsync("//LoginPage");
//        }

//        private async Task GoToLoginAsync()
//        {
//            await Shell.Current.GoToAsync("//LoginPage");
//        }
//    }
//}









































//using System.Text;
//using System.Text.Json;
//using System.Windows.Input;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using OURSTORE.Models;
//using YourApp.ViewModels;

//namespace YourApp.ViewModels
//{
//    public partial class RegisterViewModel : ObservableObject
//    {
//        [ObservableProperty] private string username;
//        [ObservableProperty] private string password;

//        public ICommand RegisterCommand => new AsyncRelayCommand(RegisterAsync);

//        private async Task RegisterAsync()
//        {
//            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
//            {
//                await Shell.Current.DisplayAlert("خطأ", "من فضلك أدخل اسم المستخدم وكلمة المرور", "تم");
//                return;
//            }

//            try
//            {
//                var user = new UserModel
//                {
//                    Username = Username,
//                    Password = Password
//                };

//                var json = JsonSerializer.Serialize(user);
//                var content = new StringContent(json, Encoding.UTF8, "application/json");

//                using var client = new HttpClient();
//                var response = await client.PostAsync("https://fakestoreapi.com/users", content);

//                if (response.IsSuccessStatusCode)
//                {
//                    await Shell.Current.DisplayAlert("تم التسجيل ✅", "تم إنشاء الحساب بنجاح", "تسجيل الدخول");
//                    await Shell.Current.GoToAsync("//LoginPage");
//                }
//                else
//                {
//                    await Shell.Current.DisplayAlert("خطأ", "فشل إنشاء الحساب، حاول مرة أخرى", "تم");
//                }
//            }
//            catch (Exception ex)
//            {
//                await Shell.Current.DisplayAlert("خطأ", ex.Message, "تم");
//            }
//        }
//    }
//}



















































using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStoreApp.Models;
using MauiStoreApp.Services;
using OURSTORE.Models;
using System.Net.Http.Json;

namespace MauiStoreApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {

        
            [ObservableProperty]
            string username;

            [ObservableProperty]
            string password;

            [RelayCommand]
            public async Task Register()
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await App.Current.MainPage.DisplayAlert("خطأ", "أدخل اسم المستخدم وكلمة المرور", "حسناً");
                    return;
                }

                try
                {
                    var user = new UserModel
                    {
                        Username = Username,
                        Password = Password
                    };

                    using var client = new HttpClient();
                    var response = await client.PostAsJsonAsync("https://fakestoreapi.com/users", user);

                    if (response.IsSuccessStatusCode)
                    {
                        await SecureStorage.Default.SetAsync("username", Username);
                        await SecureStorage.Default.SetAsync("password", Password);

                        await App.Current.MainPage.DisplayAlert("تم التسجيل", "يمكنك الآن تسجيل الدخول", "تمام");
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("خطأ", "حدث خطأ أثناء التسجيل", "حسناً");
                    }
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("خطأ", ex.Message, "حسناً");
                }
            }
        }
    }
